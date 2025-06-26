// <copyright file="ExceptionHandler.cs" company="Sascha Manns">
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
