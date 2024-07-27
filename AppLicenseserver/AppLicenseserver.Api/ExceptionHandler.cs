// <copyright file="ExceptionHandler.cs" company="marcos software">
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

using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;

namespace AppLicenseserver.Api
{
	/// <summary>
	/// Middleware - error handling.
	/// </summary>
	public class ExceptionHandler
	{
		private readonly RequestDelegate _next;

		/// <summary>
		/// Initializes a new instance of the <see cref="ExceptionHandler"/> class.
		/// </summary>
		/// <param name="next">The next.</param>
		public ExceptionHandler(RequestDelegate next)
		{
			_next = next;
		}

		/// <summary>
		/// Invokes the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns>Task with context.</returns>
		public async Task Invoke(HttpContext context)
		{
			try
			{
				await _next.Invoke(context);
			}
			catch (Exception ex)
			{
				await HandleExceptionAsync(context, ex);
				if (Startup.Configuration["Exception:ThrowExceptionAfterLog"] == "True")
				{
					throw ex;
				}
			}
		}

		/// <summary>
		/// Handles the exception asynchronous.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="exception">The exception.</param>
		private async Task HandleExceptionAsync(HttpContext context, Exception exception)
		{
			var response = context.Response;
			response.ContentType = "application/json";
			response.StatusCode = (int)HttpStatusCode.InternalServerError;

			// get inner if exists
			var innerExceptionMsg = string.Empty;
			if (exception.InnerException != null)
			{
				innerExceptionMsg = exception.InnerException.Message;
			}

			var result = JsonConvert.SerializeObject(new
			{
				// customize as you need
				error = new
				{
					message = exception.Message + Environment.NewLine + innerExceptionMsg,
					exception = exception.GetType().Name,
				},
			});
			await response.WriteAsync(result);

			Log.Error("ERROR FOUND", result);
		}
	}
}
