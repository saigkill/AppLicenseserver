// <copyright file="FakeRemoteIpAddressMiddleware.cs" company="marcos software">
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
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace AppLicenseserver.Api.Middlewares
{
	/// <summary>
	/// NOTE: this pipeline is only used when integration tests run to populate empty
	///       requests RemoteIp address required for DDoS attacks prevention test.
	/// </summary>
	public class FakeRemoteIpAddressMiddleware
	{
		private readonly RequestDelegate next;
		private IPAddress fakeIpAddress = null; // IPAddress.Parse("127.168.1.32");

		/// <summary>
		/// Initializes a new instance of the <see cref="FakeRemoteIpAddressMiddleware"/> class.
		/// </summary>
		/// <param name="next">The next.</param>
		public FakeRemoteIpAddressMiddleware(RequestDelegate next)
		{
			this.next = next;
		}

		//RemoteIpAddress=null when run from Integration test so must be set fake one for DDoS attacks prevention tests

		/// <summary>
		/// Invokes the specified HTTP context.
		/// </summary>
		/// <param name="httpContext">The HTTP context.</param>
		/// <returns>Task with httpContext.</returns>
		public async Task Invoke(HttpContext httpContext)
		{
			if (fakeIpAddress == null)
			{
				fakeIpAddress = IPAddress.Parse("127.168.1." + RandomNumber(1, 168));
			}

			httpContext.Connection.RemoteIpAddress = fakeIpAddress;

			await this.next(httpContext);
		}

		/// <summary>
		/// Generates a random number within a range.
		/// </summary>
		/// <param name="min">The minimum.</param>
		/// <param name="max">The maximum.</param>
		/// <returns>New Random number.</returns>
		public int RandomNumber(int min, int max)
		{
			return new Random().Next(min, max);
		}
	}

	/// <summary>
	/// Extension method for <see cref="IApplicationBuilder"/>.
	/// </summary>
	public static class FakeRemoteIpAddressMiddlewareExtensions
	{
		/// <summary>
		/// Uses the fake remote ip address middleware extensions.
		/// </summary>
		/// <param name="builder">The builder.</param>
		/// <returns>builder.UseMiddleware</returns>
		public static IApplicationBuilder UseFakeRemoteIpAddressMiddlewareExtensions(this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<FakeRemoteIpAddressMiddleware>();
		}
	}
}