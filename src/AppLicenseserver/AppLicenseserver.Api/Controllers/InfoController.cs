// <copyright file="InfoController.cs" company="Sascha Manns">
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
#pragma warning disable SA1629 // DocumentationTextMustEndWithAPeriod
#pragma warning disable SA1300 // ElementMustBeginWithUpperCaseLetter

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using AppLicenseserver.Api.Attributes;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace AppLicenseserver.Api.Controllers.v1
{
    /// <summary>
    /// This controller is an example of API versioning
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InfoController"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public InfoController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// The information controller for the API. It is DDos protected and allows getting information without authentication.
        /// </summary>
        /// <returns>The html website code</returns>
        [DDosAttackProtected]
        [AllowAnonymous]
        [HttpGet]
        [Produces("application/json")]
        public IActionResult ApiInfo()
        {
            var migration = Configuration["ConnectionStrings:UseMigrationService"];
            var seed = Configuration["ConnectionStrings:UseSeedService"];
            var memorydb = Configuration["ConnectionStrings:UseInMemoryDatabase"];
            var authentication = Configuration["Authentication:UseIdentityServer4"];
            var is4ip = Configuration["Authentication:IdentityServer4IP"];
            var ddosAttackProtected = Configuration["DDosAttackProtection:Enabled"];

            var controlers = MvcHelper.GetControllerMethodsNames();
            return Content(
                "<html><head><link href='https://stackpath.bootstrapcdn.com/bootstrap/4.4.1/css/bootstrap.min.css' rel='stylesheet' integrity='sha384-Vkoo8x4CGsO3+Hhxv8T/Q5PaXtkKtu6ug5TOeNV6gBiFeWPGFN9MuhOf23Q9Ifjh' crossorigin='anonymous'></head><body>" +

                "<div class='jumbotron'>" +
                "<h1><i class='fab fa-centercode' fa-2x></i>  AppLicenseserver Api v.1</h1>" +
                "<h4>Created with RestApiNEx v.6.1.0</h4>" +
                 "REST API service started!<br>" +
                 "appsettings.json configuration:<br>" +
                 "<ul><li>.NET 6.0</li>" +
                 "<li>Use Migration Service: " + migration + "</li>" +
                 "<li>Use Seed Service: " + seed + "</li>" +
                 "<li>Use InMemory Database: " + memorydb + "</li>" +
                  "<li>Authentication Type: " + (authentication == "True" ? "IS4" : "JWT") + "</li>" +
                  (authentication == "True" ? "<li>IdentityServer4IP: " + is4ip + "</li>" : string.Empty) +
                  "<li>DDoS Attack Protection: " + ddosAttackProtected + "</li>" +
                 "<li>Logs location: AppLicenseserver.Api\\Logs</li></ul>" +
                 "<a class='btn btn-outline-primary' role='button' href='/swagger'><b>Swagger API specification</b></a><br>" +
                 "<a class='btn btn-outline-warning' role='button' href='http://www.anasoft.net/restapi'><b>More instructions and more features</b></a>" +
                 "<a class='btn btn-outline-warning' role='button' href='https://www.youtube.com/channel/UC5XyWfG0nGYp7Q9buusealA'><b>YouTube instructions</b></a>" +
                "</div>" +

                "<div class='row'>" +

                "<div class='col-md-3'>" +
                "<h3>API controlers and methods</h3>" +
                "<ul>" + controlers + "</ul>" +
                "<p></p>" +
                "</div>" +
                "<div class='col-md-3'>" +
                "<h3>API services and patterns</h3>" +
                "<p><ul><li>Dependency Injection (Net Core feature) </li><li>Repository and Unit of Work Patterns</li><li>Generic services</li><li>Automapper</li><li>Sync and Async calls</li><li>Generic exception handler</li><li>Serilog logging with Console and File sinks</li><li>Seed from json objects</li><li>JWT authorization and authentication</li></ul>" +
                "</div>" +
                "<div class='col-md-3'>" +
                "<h3>API projects</h3>" +
                "<ul><li>Api</li><li>Domain</li><li>Entity</li></ul>" +
                "</div>" +

                "</div>" +
                "</body></html>",
                "text/html");
        }
    }

    /// <summary>
    /// Helper class for Mvc
    /// </summary>
    public static class MvcHelper
    {
        /// <summary>
        /// Gets the controller methods names.
        /// </summary>
        /// <returns>Info with declared methods with method names</returns>
        public static string GetControllerMethodsNames()
        {
            List<Type> cmdtypes = GetSubClasses<ControllerBase>();
            var controlersInfo = string.Empty;
            foreach (Type ctrl in cmdtypes)
            {
                var methodsInfo = string.Empty;
                const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
                MemberInfo[] methodName = ctrl.GetMethods(flags);
                foreach (MemberInfo method in methodName)
                {
                    if (method.DeclaringType.ToString() == ctrl.UnderlyingSystemType.ToString())
                    {
                        methodsInfo += "<li><i>" + method.Name.ToString() + "</i></li>";
                    }
                }

                controlersInfo += "<li>" + ctrl.Name.Replace("Controller", string.Empty) + "<ul>" + methodsInfo + "</ul></li>";
            }

            return controlersInfo;
        }

        /// <summary>
        /// Gets the sub classes.
        /// </summary>
        /// <typeparam name="T">Generic types.</typeparam>
        /// <returns>A calling assembly with all subclasses of T</returns>
        private static List<Type> GetSubClasses<T>()
        {
            return Assembly.GetCallingAssembly().GetTypes().Where(
                type => type.IsSubclassOf(typeof(T))).ToList();
        }
    }
}

namespace AppLicenseserver.Api.Controllers.v2
{
    /// <summary>
    /// V2 Version of the Info Controller
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InfoController"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public InfoController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// The information controller for the API. It is DDos protected and allows getting information without authentication. APIv2
        /// </summary>
        /// <returns>The html website code</returns>
        [AllowAnonymous]
        [HttpGet]
        [Produces("application/json")]
        public IActionResult ApiInfo()
        {
            var migration = Configuration["ConnectionStrings:UseMigrationService"];
            var seed = Configuration["ConnectionStrings:UseSeedService"];
            var memorydb = Configuration["ConnectionStrings:UseInMemoryDatabase"];
            var authentication = Configuration["Authentication:UseIdentityServer4"];
            var is4ip = Configuration["Authentication:IdentityServer4IP"];

            var controlers = MvcHelper.GetControllerMethodsNames();
            return Content(
                "<html><head><link rel='stylesheet' href='https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0-beta.2/css/bootstrap.min.css' integrity='sha384-PsH8R72JQ3SOdhVi3uxftmaW6Vc51MKb0q5P2rRUpPvrszuE4W1povHYgTpBfshb' crossorigin='anonymous'><link rel='stylesheet' href='https://use.fontawesome.com/releases/v5.3.1/css/all.css' integrity='sha384-mzrmE5qonljUremFsqc01SB46JvROS7bZs3IO2EmfFsd15uHvIt+Y8vEf7N7fWAU' crossorigin='anonymous'></head><body>" +

                "<div class='jumbotron'>" +
                "<h1><i class='fab fa-centercode' fa-2x></i>  AppLicenseserver Api v.2</h1>" +
                "<h4>Created with RestApiNEx v.5.0.3</h4>" +
                 "REST Api service started!<br>" +
                 "appsettings.json configuration:<br>" +
                 "<ul><li>.NET 5.0</li>" +
                 "<li>Use Migration Service: " + migration + "</li>" +
                 "<li>Use Seed Service: " + seed + "</li>" +
                 "<li>Use InMemory Database: " + memorydb + "</li>" +
                  "<li>Authentication Type: " + (authentication == "True" ? "IS4" : "JWT") + "</li>" +
                  (authentication == "True" ? "<li>IdentityServer4IP: " + is4ip + "</li>" : string.Empty) +
                 "<a class='btn btn-outline-primary' role='button' href='/swagger'><b>Swagger API specification</b></a>" +
                "</div>" +

                "<div class='row'>" +

                "<div class='col-md-3'>" +
                "<h3>API controlers and methods</h3>" +
                "<ul>" + controlers + "</ul>" +
                "<p></p>" +
                "</div>" +
                "<div class='col-md-3'>" +
                "<h3>API services and patterns</h3>" +
                "<p><ul><li>Dependency Injection (Net Core feature) </li><li>Repository and Unit of Work Patterns</li><li>Generic services</li><li>Automapper</li><li>Sync and Async calls</li><li>Generic exception handler</li><li>Serilog logging with Console and File sinks</li><li>Seed from json objects</li><li>JWT authorization and authentication</li></ul>" +
                "</div>" +
                "<div class='col-md-3'>" +
                "<h3>API projects</h3>" +
                "<ul><li>Api</li><li>Domain</li><li>Entity</li></ul>" +
                "</div>" +

                "</div>" +
                "</body></html>",
                "text/html");
        }
    }

    /// <summary>
    /// Helper class for Mvc APIv2
    /// </summary>
    public static class MvcHelper
    {
        /// <summary>
        /// Gets the controller methods names. APIv2
        /// </summary>
        /// <returns>Info with declared methods with method names</returns>
        public static string GetControllerMethodsNames()
        {
            List<Type> cmdtypes = GetSubClasses<ControllerBase>();
            var controlersInfo = string.Empty;
            foreach (Type ctrl in cmdtypes)
            {
                var methodsInfo = string.Empty;
                const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
                MemberInfo[] methodName = ctrl.GetMethods(flags);
                foreach (MemberInfo method in methodName)
                {
                    if (method.DeclaringType.ToString() == ctrl.UnderlyingSystemType.ToString())
                    {
                        methodsInfo += "<li><i>" + method.Name.ToString() + "</i></li>";
                    }
                }

                controlersInfo += "<li>" + ctrl.Name.Replace("Controller", string.Empty) + "<ul>" + methodsInfo + "</ul></li>";
            }

            return controlersInfo;
        }

        /// <summary>
        /// Gets the sub classes. APIv2
        /// </summary>
        /// <typeparam name="T">Generic Type.</typeparam>
        /// <returns>A calling assembly with all subclasses of T</returns>
        private static List<Type> GetSubClasses<T>()
        {
            return Assembly.GetCallingAssembly().GetTypes().Where(
                type => type.IsSubclassOf(typeof(T))).ToList();
        }
    }
}
