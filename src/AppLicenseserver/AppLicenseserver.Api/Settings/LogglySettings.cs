// <copyright file="LogglySettings.cs" company="Sascha Manns">
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
