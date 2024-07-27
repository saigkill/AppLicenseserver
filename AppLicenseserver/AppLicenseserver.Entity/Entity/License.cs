// <copyright file="License.cs" company="marcos software">
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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppLicenseserver.Entity
{
	/// <summary>
	/// Entity for Serials. A user can have multiple Serials (one per product). It contains
	/// the Properties Serialnumber, UserId, ProductId.
	/// </summary>
	[System.ComponentModel.Description("Entity for Serials. A user can have multiple Serials (one per product). It contains the Properties Licensenumber, UserId, ProductId.")]
	public class License : BaseEntity
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="License"/> class.
		/// </summary>
		public License()
		{
			// Licensenumber = Guid.NewGuid();
		}

		// Properties

		/// <summary>
		/// Gets or sets the licensenumber.
		/// </summary>
		/// <value>
		/// The licensenumber.
		/// </value>
		[Required]
		[MaxLength(50)]
		[StringLength(50)]
		[Column(TypeName = "varchar(50)")]
		[System.ComponentModel.Description("Licensenumber will be generated automatically")]
		public Guid Licensenumber { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is active.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
		/// </value>
		public bool IsActive { get; set; }

		/// <summary>
		/// Gets or sets the product identifier.
		/// </summary>
		/// <value>
		/// The product identifier.
		/// </value>
		[Required]
		[System.ComponentModel.Description("ProductId used by Product")]
		[ForeignKey("Product")]
		public int ProductId { get; set; }

		/// <summary>
		/// Gets or sets the product.
		/// </summary>
		/// <value>
		/// The product.
		/// </value>
		public virtual Product Product { get; set; }

		/// <summary>
		/// Gets or sets the UserId (the foreign key to the User entity).
		/// </summary>
		[Required]
		[ForeignKey("User")]
		[System.ComponentModel.Description("UserId used by User")]
		public int UserId { get; set; }

		/// <summary>
		/// Gets or sets the user.
		/// </summary>
		/// <value>
		/// The user.
		/// </value>
		public virtual User User { get; set; }
	}
}
