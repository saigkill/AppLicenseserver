// <copyright file="GuidHelper.cs" company="marcos software">
//   this file may not be redistributed in whole or significant part
//   and is subject to the marcos software license.
//
//   @author: Sascha Manns, <s.manns@marcossoftware.com>
//   @copyright: 2022, marcos-software.de, http://www.marcos-software.de
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
