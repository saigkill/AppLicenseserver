// <copyright file="Program.cs" company="Sascha Manns">
// Copyright (c) 2025 Sascha Manns.
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
// associated documentation files (the “Software”), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial
// portions of the Software.
//
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
// PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN
// ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

#pragma warning disable SA1027 // TabsMustNotBeUsed
#pragma warning disable SA1200 // UsingDirectivesMustBePlacedWithinNamespace
#pragma warning disable SA1309 // FieldNamesMustNotBeginWithUnderscore
#pragma warning disable SA1101 // PrefixLocalCallsWithThis
#pragma warning disable SA1124 // DoNotUseRegions
#pragma warning disable SA1123 // DoNotUseRegions

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

using AppLicenseserver.Api;
using AppLicenseserver.Api.Middlewares;
using AppLicenseserver.Api.Settings;
using AppLicenseserver.Api.Utilities;
using AppLicenseserver.Domain.Mapping;
using AppLicenseserver.Domain.Service;
using AppLicenseserver.Entity.Context;
using AppLicenseserver.Entity.UnitofWork;

using AutoMapper;

using Loggly;
using Loggly.Config;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using Serilog;

using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);
var startup = new Startup(builder.Configuration);

#region "Attach services"

try
{
    #region "Add basic services"

    builder.Services.AddControllers();

    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders =
            ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    });

    // Use lowercase for all endpoints
    builder.Services.AddRouting(options => options.LowercaseUrls = true);
    #endregion

    #region "API versioning"

    // API versioning service
    builder.Services.AddApiVersioning(
        o =>
        {
            //o.Conventions.Controller<UserController>().HasApiVersion(1, 0);
            o.AssumeDefaultVersionWhenUnspecified = true;
            o.ReportApiVersions = true;
            o.DefaultApiVersion = new ApiVersion(1, 0);
            o.ApiVersionReader = new UrlSegmentApiVersionReader();
        });

    // format code as "'v'major[.minor][-status]"
    builder.Services.AddVersionedApiExplorer(
    options =>
    {
        options.GroupNameFormat = "'v'VVV";

        // versioning by url segment
        options.SubstituteApiVersionInUrl = true;
    });
    #endregion

    #region "DB service"
    if (builder.Configuration["ConnectionStrings:UseInMemoryDatabase"] == "True")
    {
        builder.Services.AddDbContext<DefaultDbContext>(opt => opt.UseInMemoryDatabase("TestDB-" + Guid.NewGuid().ToString()));
    }
    else
    {
        builder.Services.AddDbContext<DefaultDbContext>(options => options.UseSqlServer(builder.Configuration["ConnectionStrings:AppLicenseserverDB"]));
    }
    #endregion

    #region "Authentication"
    if (builder.Configuration["Authentication:UseIdentityServer4"] == "False")
    {
        // JWT API authentication service
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Issuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
            };
        });
    }
    else
    {
        // Identity Server 4 API authentication service
        builder.Services.AddAuthorization();
        // .AddJsonFormatters();
        builder.Services.AddAuthentication("Bearer")
        .AddIdentityServerAuthentication(option =>
        {
            option.Authority = builder.Configuration["Authentication:IdentityServer4IP"];
            option.RequireHttpsMetadata = false;

            // option.ApiSecret = "secret";
            option.ApiName = "AppLicenseserver";  //This is the resourceAPI that we defined in the Config.cs in the AuthServ project (apiresouces.json and clients.json). They have to be named equal.
        });

    }
    #endregion

    #region "CORS"

    // include support for CORS
    // More often than not, we will want to specify that our API accepts requests coming from other origins (other domains). When issuing AJAX requests, browsers make preflights to check if a server accepts requests from the domain hosting the web app. If the response for these preflights don't contain at least the Access-Control-Allow-Origin header specifying that accepts requests from the original domain, browsers won't proceed with the real requests (to improve security).
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(
            "CorsPolicy-public",
            builder => builder.AllowAnyOrigin() // WithOrigins and define a specific origin to be allowed (e.g. https://mydomain.com)
                .AllowAnyMethod()
                .AllowAnyHeader()

        // .AllowCredentials()
        .Build());
    });
    #endregion

    #region "DDoS attack service"
    if (builder.Configuration["DDosAttackProtection:Enabled"] == "True")
        builder.Services.AddSingleton(typeof(DDosAttackMonitoringService));
    #endregion

    #region "MVC and JSON options"

    // mvc service (set to ignore ReferenceLoopHandling in json serialization like Users[0].Account.Users)
    // in case you need to serialize entity children use commented out option instead
    builder.Services.AddMvc(option => option.EnableEndpointRouting = false)
.AddNewtonsoftJson(options => { options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore; });  // NO entity classes' children serialization
                                                                                                                                      // .AddNewtonsoftJson(ops =>
                                                                                                                                      //{
                                                                                                                                      //    ops.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize;
                                                                                                                                      //    ops.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;                                                                                                                                     //}); //WITH entity classes' children serialization
    #endregion

    #region "DI code"

    // general unitofwork injections
    builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();

    // services injections
    builder.Services.AddTransient(typeof(AccountService<,>), typeof(AccountService<,>));
    builder.Services.AddTransient(typeof(UserService<,>), typeof(UserService<,>));
    builder.Services.AddTransient(typeof(AccountServiceAsync<,>), typeof(AccountServiceAsync<,>));
    builder.Services.AddTransient(typeof(UserServiceAsync<,>), typeof(UserServiceAsync<,>));

    startup.DIServicesBuilt(builder.Services);

    // ...add other services
    builder.Services.AddTransient(typeof(IService<,>), typeof(GenericService<,>));
    builder.Services.AddTransient(typeof(IServiceAsync<,>), typeof(GenericServiceAsync<,>));
    #endregion

    #region"Data mapper services configuration"

    var mapperConfig = new MapperConfiguration(cfg =>
    {
        cfg.AddProfile<MappingProfile>();
    });
    IMapper mapper = mapperConfig.CreateMapper();
    builder.Services.AddSingleton(mapper);

    #endregion

    #region "Swagger API"

    // Swagger API documentation
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "App-Licenseserver API",
            Description = "This API is used to manage Apps, Licenses and Users. Its for internal use only.",

            // TermsOfService = new Uri("https://example.com/terms"),
            Contact = new OpenApiContact
            {
                Name = "Marcos Software Support",
                Url = new Uri("https://www.unicorn2.de/ueber-uns/kontakt/"),
                Email = "support@marcossoftware.com",
            },
        });

        c.SwaggerDoc("v2", new OpenApiInfo
        {
            Version = "v2",
            Title = "App-Licenseserver API",
            Description = "This API is used to manage Apps, Licenses and Users. Its for internal use only.",
            // TermsOfService = new Uri("https://example.com/terms"),
            Contact = new OpenApiContact
            {
                Name = "Marcos Software Support",
                Url = new Uri("https://www.unicorn2.de/ueber-uns/kontakt/"),
                Email = "support@marcossoftware.com"
            },
        });

        // In Test project find attached swagger.auth.pdf file with instructions how to run Swagger authentication.
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
        {
            Description = "Authorization header using the Bearer scheme",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
        });


        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme {
                            Reference = new OpenApiReference {

                                Id = "Bearer", // The name of the previously defined security scheme.
								Type = ReferenceType.SecurityScheme,
                            },
                }, new List<string>()
            },
        });

        // c.DocumentFilter<api.infrastructure.filters.SwaggerSecurityRequirementsDocumentFilter>();

        // using System.Reflection;
        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    });

    #endregion

    #region Application Insights

    builder.Services.AddApplicationInsightsTelemetry(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]);

    #endregion
}
catch (Exception ex)
{
    Log.Error(ex.Message);
}

#endregion

#region "Build web app"

var app = builder.Build();

#region "Read configurationa and setup logger"

string env = app.Environment.EnvironmentName;
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
            .Build();

// Must have Loggly account and setup correct info in appsettings
if (configuration["Serilog:UseLoggly"] == "true")
{
    var logglySettings = new LogglySettings();
    configuration.GetSection("Serilog:Loggly").Bind(logglySettings);
    Startup.SetupLogglyConfiguration(logglySettings);
}

Log.Logger = new LoggerConfiguration()
.ReadFrom.Configuration(configuration)
.CreateLogger();

#endregion

// Forwarded Headers Middleware should run before other middleware. This ordering ensures that the middleware relying on forwarded headers information can consume the header values for processing.
app.UseForwardedHeaders();

if (app.Environment.EnvironmentName == "Development")
    app.UseDeveloperExceptionPage();
else
{
    app.UseMiddleware<ExceptionHandler>();
    // app.UseHsts();
}

// app.UseHttpsRedirection();
// app.UseStaticFiles();
// app.UseCookiePolicy();
// app.UseRequestLocalization();

app.UseRouting();
app.UseAuthentication(); // needs to be up in the pipeline, before MVC
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/", async http =>
    {
        http.Response.Redirect("api/v1/info", true);
    });

    // add more get. post, put routing here
});

app.UseCors("CorsPolicy-public");  // apply to every request

// app.UseSession();
// app.UseResponseCompression();
// app.UseResponseCaching()

// Swagger API documentation
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "AppLicenseserver API V1");
    c.SwaggerEndpoint("/swagger/v2/swagger.json", "AppLicenseserver API V2");
    c.DisplayOperationId();
    c.DisplayRequestDuration();
});

// Middlewares (orders of all middlewares(including custom) is very important)
// 1
// NOTE:  this pipeline (1) is only used when integration tests run to populate empty
//       requests RemoteIp address required for DDoS attacks prevention test
if (builder.Configuration["IntegrationTests"] == "True")
    app.UseFakeRemoteIpAddressMiddlewareExtensions();

// 2
if (builder.Configuration["DDosAttackProtection:Enabled"] == "True")
    app.UseDDosAttackStopMiddlewareExtensions();

app.UseMvc();

#endregion

#region "Migrations and seeds from json files"
using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    if (builder.Configuration["ConnectionStrings:UseInMemoryDatabase"] == "False" && !serviceScope.ServiceProvider.GetService<DefaultDbContext>().AllMigrationsApplied())
    {
        if (builder.Configuration["ConnectionStrings:UseMigrationService"] == "True")
            serviceScope.ServiceProvider.GetService<DefaultDbContext>().Database.Migrate();
    }

    // it will seed tables on aservice run from json files if tables empty
    if (builder.Configuration["ConnectionStrings:UseSeedService"] == "True")
        serviceScope.ServiceProvider.GetService<DefaultDbContext>().EnsureSeeded();
}
#endregion

#region "Start app"
try
{
    Log.Information("Starting web REST API application");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Web REST API application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

// .NET 6 compiler generates the Program class behind the scenes as the internal class, thus making it inaccessible in our integration testing project. So to solve this, we can create a public partial Program class in the Program.cs file

/// <summary>
/// The main entry point for the application.
/// </summary>
public partial class Program
{
}
#endregion

namespace AppLicenseserver.Api
{
    /// <summary>
    /// Startup Class.
    /// </summary>
    public partial class Startup
    {
        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public static Microsoft.Extensions.Configuration.IConfiguration Configuration { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public Startup(Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Dis the services built.
        /// </summary>
        /// <param name="services">The services.</param>
        public void DIServicesBuilt(IServiceCollection services)
        {
            SetAdditionalDIServices(services);
        }

        // call scaffolded class method to add DIs
        partial void SetAdditionalDIServices(IServiceCollection services);

        /// <summary>
        /// Configure Loggly provider.
        /// </summary>
        /// <param name="logglySettings">A LogglySettings object.</param>
        public static void SetupLogglyConfiguration(LogglySettings logglySettings)
        {
            // Configure Loggly
            var config = LogglyConfig.Instance;
            config.CustomerToken = logglySettings.CustomerToken;
            config.ApplicationName = logglySettings.ApplicationName;
            config.Transport = new TransportConfiguration()
            {
                EndpointHostname = logglySettings.EndpointHostname,
                EndpointPort = logglySettings.EndpointPort,
                LogTransport = logglySettings.LogTransport,
            };
            config.ThrowExceptions = logglySettings.ThrowExceptions;

            // Define Tags sent to Loggly
            config.TagConfig.Tags.AddRange(new ITag[]
            {
                new ApplicationNameTag { Formatter = "Application-{0}" },
                new HostnameTag { Formatter = "Host-{0}" },
            });
        }
    }
}

namespace api.infrastructure.filters
{
    /// <summary>
    /// Swagger Security Filter.
    /// </summary>
    /// <seealso cref="Swashbuckle.AspNetCore.SwaggerGen.IDocumentFilter" />
    public class SwaggerSecurityRequirementsDocumentFilter : IDocumentFilter
    {
        /// <summary>
        /// Applies the specified document.
        /// </summary>
        /// <param name="swaggerDoc">The document.</param>
        /// <param name="context">The context.</param>
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            swaggerDoc.SecurityRequirements = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer", // The name of the previously defined security scheme.
								Type = ReferenceType.SecurityScheme,
                            },
                        }, new List<string>()
                    },
                },
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Basic", // The name of the previously defined security scheme.
								Type = ReferenceType.SecurityScheme,
                            },
                        }, new List<string>()
                    },
                },
            };
        }
    }
}
