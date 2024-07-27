// <copyright file="BaseDomain.cs" company="marcos software">
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

namespace AppLicenseserver.Domain
{
	/// <summary>
	/// The definitions of Base Domain.
	/// </summary>
	public class BaseDomain
	{
		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>
		/// The identifier.
		/// </value>
		public int Id { get; set; }

		/// <summary>
		/// Gets or sets the row version.
		/// </summary>
		/// <value>
		/// The row version.
		/// </value>
		public byte[] RowVersion { get; set; }

		/// <summary>
		/// Gets or sets the test text. String item for T4 generated tests.
		/// </summary>
		/// <value>
		/// The test text.
		/// </value>
		public string TestText { get; set; }
	}
}
