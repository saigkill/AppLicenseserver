// <copyright file="BaseEntity.cs" company="marcos software">
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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppLicenseserver.Entity
{
	/// <summary>
	/// Base Entity where all other entities derived from.
	/// </summary>
	public class BaseEntity
	{
		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>
		/// The identifier.
		/// </value>
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Key]
		public int Id { get; set; }

		/// <summary>
		/// Gets or sets the created.
		/// </summary>
		/// <value>
		/// The created.
		/// </value>
		public DateTime Created { get; set; }

		/// <summary>
		/// Gets or sets the modified.
		/// </summary>
		/// <value>
		/// The modified.
		/// </value>
		public DateTime Modified { get; set; }

		// [ConcurrencyCheck]
		// [Timestamp]

		/// <summary>
		/// Gets or sets the row version.
		/// </summary>
		/// <value>
		/// The row version.
		/// </value>
		public byte[] RowVersion { get; set; }

		/// <summary>
		/// Gets or sets the test text. (string item for T4 generated tests).
		/// </summary>
		/// <value>
		/// The test text.
		/// </value>
		[StringLength(50)]
		public string TestText { get; set; }
	}
}
