// <copyright file="LogglySettings.cs" company="marcos software">
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

using Loggly.Config;

namespace AppLicenseserver.Api.Settings
{
	/// <summary>
	/// Settings Class for Loggly.
	/// </summary>
	public class LogglySettings
	{
		/// <summary>
		/// Gets or sets the name of the application.
		/// </summary>
		/// <value>
		/// The name of the application.
		/// </value>
		public string ApplicationName { get; set; }

		/// <summary>
		/// Gets or sets the account.
		/// </summary>
		/// <value>
		/// The account.
		/// </value>
		public string Account { get; set; }

		/// <summary>
		/// Gets or sets the username.
		/// </summary>
		/// <value>
		/// The username.
		/// </value>
		public string Username { get; set; }

		/// <summary>
		/// Gets or sets the password.
		/// </summary>
		/// <value>
		/// The password.
		/// </value>
		public string Password { get; set; }

		/// <summary>
		/// Gets or sets the endpoint port.
		/// </summary>
		/// <value>
		/// The endpoint port.
		/// </value>
		public int EndpointPort { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is enabled; otherwise, <c>false</c>.
		/// </value>
		public bool IsEnabled { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [throw exceptions].
		/// </summary>
		/// <value>
		///   <c>true</c> if [throw exceptions]; otherwise, <c>false</c>.
		/// </value>
		public bool ThrowExceptions { get; set; }

		/// <summary>
		/// Gets or sets the log transport.
		/// </summary>
		/// <value>
		/// The log transport.
		/// </value>
		public LogTransport LogTransport { get; set; }

		/// <summary>
		/// Gets or sets the endpoint hostname.
		/// </summary>
		/// <value>
		/// The endpoint hostname.
		/// </value>
		public string EndpointHostname { get; set; }

		/// <summary>
		/// Gets or sets the customer token.
		/// </summary>
		/// <value>
		/// The customer token.
		/// </value>
		public string CustomerToken { get; set; }
	}
}
