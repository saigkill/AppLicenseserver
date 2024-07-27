// <copyright file="DDosAttackStopMiddleware.cs" company="marcos software">
//   this file may not be redistributed in whole or significant part
//   and is subject to the marcos software license.
//
//   @author: Sascha Manns, s.manns@marcossoftware.com
//   @copyright: 2022, marcos-software.de, http://www.marcos-software.de
// </copyright>

#pragma warning disable SA1027 // TabsMustNotBeUsed
#pragma warning disable SA1200 // UsingDirectivesMustBePlacedWithinNamespace
#pragma warning disable SA1309 // FieldNamesMustNotBeginWithUnderscore
#pragma warning disable SA1101 // PrefixLocalCallsWithThis

using System.Net;
using System.Threading.Tasks;
using AppLicenseserver.Api.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

// Warning! If you have a Load Balancer, then you must to configure the Load Balancer to send
//         the original Client's IP as the Request IP to the webserver. Another option is to pass the
//         original Client IP in a X-Forwarded-For header.
//         RequestHandler maintains its counters using the Client IP. If the Client IP is the Load Balancer's IP,
//         not the original Client's IP, then it will lock out the load balancer, causing total
//         outage on your website.
// https://docs.microsoft.com/en-us/aspnet/core/migration/http-modules?view=aspnetcore-5.0


namespace AppLicenseserver.Api.Middlewares
{
	/// <summary>
	/// Middleware to stop DDOS attacks.
	/// </summary>
	public class DDosAttackStopMiddleware
	{
		private const string XForwardedForHeader = "X-Forwarded-For";

		private readonly RequestDelegate _next;

		private readonly DDosAttackMonitoringService _DDosAttackMonitoringService;

		/// <summary>
		/// Initializes a new instance of the <see cref="DDosAttackStopMiddleware"/> class.
		/// </summary>
		/// <param name="next">The next.</param>
		/// <param name="dDosAttackMonitoringService">The d dos attack monitoring service.</param>
		public DDosAttackStopMiddleware(RequestDelegate next, DDosAttackMonitoringService dDosAttackMonitoringService)
		{
			_next = next;
			_DDosAttackMonitoringService = dDosAttackMonitoringService;
		}

		/// <summary>
		/// Invokes the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns>Invoked context.</returns>
		public async Task Invoke(HttpContext context)
		{
			// Before request processing.

			// DDosAttackMonitoringService
			if (_DDosAttackMonitoringService.Enabled)
			{
				if (IsPathDDosAttackMonitored(context)) // check if api match attributed method
				{
					var ipAddress = context.Request?.HttpContext?.Connection?.RemoteIpAddress?.MapToIPv4();
					if (context.Request.Headers.ContainsKey(XForwardedForHeader)) // load balancers passing the original client IP
					{
						ipAddress = IPAddress.Parse(context.Request.Headers[XForwardedForHeader]).MapToIPv4();
					}

					var terminateRequest = await _DDosAttackMonitoringService.IsDDosAttack(ipAddress);
					if (terminateRequest)
					{
						context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
						await context.Response.WriteAsync("TooManyHits");
					}
					else
					{
						await _next.Invoke(context);
					}
				}
				else
				{
					await _next.Invoke(context);
				}
			}
			else
			{
				await _next.Invoke(context);
			}

			// Clean up.
		}

		/// <summary>
		/// Determines if the given path is monitored for DDos attacks.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns>
		///   <c>true</c> if [is path d dos attack monitored] [the specified context]; otherwise, <c>false</c>.
		/// </returns>
		private bool IsPathDDosAttackMonitored(HttpContext context)
		{
			if (!context.Request.Path.HasValue)
			{
				return false;
			}

			if (_DDosAttackMonitoringService.FullServiceLevelProtection)
			{
				return true;
			}

			var reqPath = context.Request.Path.ToString().ToLower();
			var reqMethod = context.Request.Method.ToString().ToUpper();

			reqPath = reqPath.Replace(@"\", "/");
			reqPath = reqPath.Substring(reqPath.IndexOf("api/") + 4);
			string reqMethodPath = reqMethod + " " + reqPath;

			// NOTE: protection at HTTP Controller/method level
			foreach (string httpMethodController in _DDosAttackMonitoringService.ProtectedCalls)
			{
				if (reqMethodPath.Contains(httpMethodController))
				{
					return true;
				}
			}

			return false;
		}

		// private string GetClientIP(HttpContext context)
		// {
		//    string ipList = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

		//    if (!string.IsNullOrEmpty(ipList))
		//    {
		//        return ipList.Split(',')[0];
		//    }

		//    return request.ServerVariables["REMOTE_ADDR"];
		//}
	}

	/// <summary>
	/// Extension method to add the middleware to the HTTP request pipeline.
	/// </summary>
	public static class DDosAttackStopMiddlewareExtensions
	{
		/// <summary>
		/// Uses the d dos attack stop middleware extensions.
		/// </summary>
		/// <param name="builder">The builder.</param>
		/// <returns>builder.UseMiddleware.</returns>
		public static IApplicationBuilder UseDDosAttackStopMiddlewareExtensions(this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<DDosAttackStopMiddleware>();
		}
	}
}