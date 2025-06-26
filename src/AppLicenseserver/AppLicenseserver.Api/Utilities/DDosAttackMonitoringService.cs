// <copyright file="DDosAttackMonitoringService.cs" company="Sascha Manns">
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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;

using AppLicenseserver.Api.Attributes;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;

using Serilog;


namespace AppLicenseserver.Api.Utilities
{
    /// <summary>
    /// Based on code from
    /// https://www.madskristensen.net/ .
    /// </summary>
    public class DDosAttackMonitoringService
    {
        private static Dictionary<string, short> _IpAdresses = new Dictionary<string, short>();
        private static Stack<string> _Banned = new Stack<string>();
        private static Timer _Timer;
        private static Timer _BannedTimer;

        // default
        private static bool _FullServiceLevelProtection = false;
        private static bool _Enabled = false;
        private static int _MaxHitsPerOrigin = 100;
        private static int _MaxHitsPerOriginIntervalMs = 1000;
        private static int _ReleaseIntervalMs = 600000;

        // find and save protected controller/methods
        // private static Dictionary<string, MethodInfo> _ProtectedMethods;
        private static List<string> _ProtectedCalls = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DDosAttackMonitoringService"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public DDosAttackMonitoringService(IConfiguration configuration)
        {
            _Enabled = configuration["DDosAttackProtection:Enabled"] == "True";
            _FullServiceLevelProtection = configuration["DDosAttackProtection:FullServiceLevelProtection"] == "True";
            int.TryParse(configuration["DDosAttackProtection:MaxHitsPerOrigin"], out _MaxHitsPerOrigin);
            int.TryParse(configuration["DDosAttackProtection:MaxHitsPerOriginIntervalMs"], out _MaxHitsPerOriginIntervalMs);
            int.TryParse(configuration["DDosAttackProtection:ReleaseIntervalMs"], out _ReleaseIntervalMs);

            // extract attributed Rest Api action calls
            foreach (MethodInfo mi in Assembly.GetExecutingAssembly().GetTypes().SelectMany(x => x.GetMethods()).Where(y => y.GetCustomAttributes().OfType<DDosAttackProtectedAttribute>().Any()))
            {
                HttpMethodAttribute httpMethodAttribute = (HttpMethodAttribute)GetWebApiMethodAttribute(mi);
                string httpMethodName = httpMethodAttribute.HttpMethods.ToList()[0].ToUpper();
                string controllerName = mi.DeclaringType.Name.Replace("Controller", string.Empty).ToString().ToLower();
                string actionName = string.Empty;
                string template = httpMethodAttribute.Template;
                if (!string.IsNullOrEmpty(template) && template.Length > 0)
                {
                    string httpTemplate = httpMethodAttribute.Template.Replace(@"\", "/");
                    // actionName = template.Substring(0, template.IndexOf("/{"));
                    var v = new Regex("^(.+?)/{.+}").Match(httpTemplate);
                    if (v.Success)
                    {
                        actionName = v.Groups[1].ToString();
                    }
                }

                string apiDefinition = httpMethodName + " " + controllerName;
                if (!string.IsNullOrEmpty(actionName))
                {
                    apiDefinition += "/" + actionName.ToLower();
                }

                _ProtectedCalls.Add(apiDefinition);
            }

            _Timer = CreateTimer();
            _BannedTimer = CreateBanningTimer();
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="DDosAttackMonitoringService"/> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        public Boolean Enabled
        {
            get { return _Enabled; }
        }

        /// <summary>
        /// Gets a value indicating whether [full service level protection].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [full service level protection]; otherwise, <c>false</c>.
        /// </value>
        public Boolean FullServiceLevelProtection
        {
            get { return _FullServiceLevelProtection; }
        }

        /// <summary>
        /// Gets the protected calls.
        /// </summary>
        /// <value>
        /// The protected calls.
        /// </value>
        public List<string> ProtectedCalls
        {
            get { return _ProtectedCalls; }
        }

        /// <summary>
        /// Determines whether [is d dos attack] [the specified ip].
        /// </summary>
        /// <param name="ip">The ip.</param>
        /// <returns>
        ///   <c>true</c> if [is d dos attack] [the specified ip]; otherwise, <c>false</c>.
        /// </returns>
        public virtual async Task<bool> IsDDosAttack(IPAddress ip)
        {
            await CheckIpAddress(ip.ToString());

            // check if ip banned
            return _Banned.Contains(ip.ToString());
        }


        /// <summary>
        /// Checks the requesting IP address in the collection
        /// and bannes the IP if required.
        /// </summary>
        private static async Task CheckIpAddress(string ip)
        {
            if (!_IpAdresses.ContainsKey(ip))
            {
                _IpAdresses[ip] = 1;
            }
            else if (_IpAdresses[ip] == _MaxHitsPerOrigin)
            {
                if (!_Banned.Contains(ip))
                    _Banned.Push(ip);
                _IpAdresses.Remove(ip);
                Log.Warning("DDosAttackStop:Blacklisted IP:" + ip + " after " + _MaxHitsPerOrigin.ToString() + " hits in " + _MaxHitsPerOriginIntervalMs.ToString() + " miliseconds");
            }
            else
            {
                _IpAdresses[ip]++;
            }
        }

        // Timers

        /// <summary>
        /// Creates the timer that substract a request
        /// from the _IpAddress dictionary.
        /// </summary>
        private static Timer CreateTimer()
        {
            Timer timer = GetTimer(_MaxHitsPerOriginIntervalMs);
            timer.Elapsed += new ElapsedEventHandler(TimerElapsed);
            return timer;
        }

        /// <summary>
        /// Creates the timer that removes 1 banned IP address
        /// everytime the timer is elapsed.
        /// </summary>
        /// <returns>timer.</returns>
        private static Timer CreateBanningTimer()
        {
            Timer timer = GetTimer(_ReleaseIntervalMs);
            timer.Elapsed += delegate
            {
                if (_Banned.Any())
                {
                    var ip = _Banned.Pop();
                    Log.Warning("DDosAttackStop:Whitelisted IP:" + ip + " after " + _ReleaseIntervalMs.ToString() + " miliseconds");
                }
            };
            return timer;
        }

        /// <summary>
        /// Creates a simple timer instance and starts it.
        /// </summary>
        /// <param name="interval">The interval in milliseconds.</param>
        private static Timer GetTimer(int interval)
        {
            Timer timer = new Timer();
            timer.Interval = interval;
            timer.Start();

            return timer;
        }

        /// <summary>
        /// Substracts a request from each IP address in the collection.
        /// </summary>
        private static void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            foreach (string key in _IpAdresses.Keys)
            {
                _IpAdresses[key]--;
                if (_IpAdresses[key] == 0)
                {
                    _ = _IpAdresses.Remove(key);
                }
            }
        }

        private static Attribute? GetWebApiMethodAttribute(MethodInfo methodInfo)
        {
            List<Attribute> attribs = methodInfo.GetCustomAttributes().Where(attr =>
               attr.GetType() == typeof(HttpGetAttribute)
               || attr.GetType() == typeof(HttpPutAttribute)
               || attr.GetType() == typeof(HttpPostAttribute)
               || attr.GetType() == typeof(HttpDeleteAttribute)
                ).AsEnumerable().ToList();

            if (attribs.Any())
            {
                return attribs[0];
            }
            else
            {
                return null;
            }
        }
    }
}
