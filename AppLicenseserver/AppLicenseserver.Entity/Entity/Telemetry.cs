// <copyright file="Telemetry.cs" company="marcos software">
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

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppLicenseserver.Entity
{
	/// <summary>
	/// Our Telemetry table. It contains Serialnumber, Ip-Number, ProductId and UserId.
	/// </summary>
	[System.ComponentModel.Description("Our Telemetry table. It contains Serialnumber, Ip-Number, ProductId and UserId.")]
	public class Telemetry : BaseEntity
	{
		// Properties

		/// <summary>
		/// Gets or sets customers Ip address.
		/// </summary>
		[Required]
		[MaxLength(16)]
		[StringLength(16)]
		[System.ComponentModel.Description("Users current IP Address")]
		public string Ip { get; set; }

		/// <summary>
		/// Gets or sets the ProductId (Foreign Key of the product).
		/// </summary>
		[Required]
		[ForeignKey("Product")]
		[System.ComponentModel.Description("Users current used product")]
		public int ProductId { get; set; }

		/// <summary>
		/// Gets or sets the product.
		/// </summary>
		/// <value>
		/// The product.
		/// </value>
		public virtual Product Product { get; set; }

		/// <summary>
		/// Gets or sets the LicenseId.
		/// </summary>
		[Required]
		[MaxLength(50)]
		[StringLength(50)]
		[ForeignKey("License")]
		[System.ComponentModel.Description("Users used License")]
		public int LicenseId { get; set; }

		/// <summary>
		/// Gets or sets the license.
		/// </summary>
		/// <value>
		/// The license.
		/// </value>
		public virtual License License { get; set; }

		/// <summary>
		/// Gets or sets the UserId (Foreign Key of the user).
		/// </summary>
		[Required]
		[ForeignKey("User")]
		[System.ComponentModel.Description("Users Id")]
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
