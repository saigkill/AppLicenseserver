// <copyright file="Product.cs" company="marcos software">
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
using System.Collections.Generic;

namespace AppLicenseserver.Entity
{
	/// <summary>
	/// Our Product table. It contains all apps, what are released.
	/// </summary>
	[System.ComponentModel.Description("Our Product table. It contains all apps, what are released.")]
	public class Product : BaseEntity
	{
		// Properties

		/// <summary>
		/// Gets or sets the Name of the product.
		/// </summary>
		[System.ComponentModel.Description("Name of the product.")]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets Description of the product.
		/// </summary>
		[System.ComponentModel.Description("Description of the product.")]
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the Version of the product.
		/// </summary>
		[System.ComponentModel.Description("Version of the product.")]
		public string Version { get; set; }

		/// <summary>
		/// Gets or sets the Release date of the product.
		/// </summary>
		[System.ComponentModel.Description("Release date of the product.")]
		public DateTime ReleaseDate { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the product is released or not.
		/// </summary>
		[System.ComponentModel.Description("The product is released or not.")]
		public bool IsReleased { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the product is active or not.
		/// </summary>
		[System.ComponentModel.Description("The product is active or not.")]
		public bool IsActive { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the product is deleted or not.
		/// </summary>
		[System.ComponentModel.Description("The product is deleted or not.")]
		public bool IsDeleted { get; set; }

		// Navigation Properties

		/// <summary>
		/// Gets or sets the licenses.
		/// </summary>
		/// <value>
		/// The licenses.
		/// </value>
		public virtual ICollection<License> Licenses { get; set; }

		/// <summary>
		/// Gets or sets the telemetries.
		/// </summary>
		/// <value>
		/// The telemetries.
		/// </value>
		public virtual ICollection<Telemetry> Telemetries { get; set; }
	}
}