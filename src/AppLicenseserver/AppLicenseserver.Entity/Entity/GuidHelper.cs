// <copyright file="GuidHelper.cs" company="Sascha Manns">
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

namespace AppLicenseserver.Entity
{
    /// <summary>
    /// Helper Class for generating a GUID (used as serial).
    /// </summary>
    public static class GuidHelper
    {
        /// <summary>
        /// Generates a GUID (used as serial).
        /// </summary>
        /// <returns>Generated GUID.</returns>
        public static Guid Generate()
        {
            var buffer = Guid.NewGuid().ToByteArray();

            var time = new DateTime(0x76c, 1, 1);
            var now = DateTime.Now;
            var span = new TimeSpan(now.Ticks - time.Ticks);
            var timeOfDay = now.TimeOfDay;

            var bytes = BitConverter.GetBytes(span.Days);
            var array = BitConverter.GetBytes(
                (long)(timeOfDay.TotalMilliseconds / 3.333333));

            Array.Reverse(bytes);
            Array.Reverse(array);
            Array.Copy(bytes, bytes.Length - 2, buffer, buffer.Length - 6, 2);
            Array.Copy(array, array.Length - 4, buffer, buffer.Length - 4, 4);

            return new Guid(buffer);
        }
    }
}
