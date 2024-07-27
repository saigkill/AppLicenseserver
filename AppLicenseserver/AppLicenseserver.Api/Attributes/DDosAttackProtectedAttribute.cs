// <copyright file="DDosAttackProtectedAttribute.cs" company="marcos software">
//   this file may not be redistributed in whole or significant part
//   and is subject to the marcos software license.
//
//   @author: Sascha Manns, s.manns@marcossoftware.com
//   @copyright: 2022, marcos-software.de, http://www.marcos-software.de
// </copyright>

//#pragma warning disable SA1027 // TabsMustNotBeUsed
//#pragma warning disable SA1200 // UsingDirectivesMustBePlacedWithinNamespace

using System;

namespace AppLicenseserver.Api.Attributes
{
	/// <summary>
	///    This attribute prevents the DDos attack.
	/// </summary>
	/// <seealso cref="System.Attribute" />
	[AttributeUsage(AttributeTargets.Method)]
	public class DDosAttackProtectedAttribute : Attribute
	{
	}
}
