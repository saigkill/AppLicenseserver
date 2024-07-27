// <copyright file="Account.cs" company="marcos software">
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
using System.ComponentModel.DataAnnotations;

namespace AppLicenseserver.Entity
{
	/// <summary>
	/// Entity for Companies. Each Company should have only one Account. It contains the
	/// Properties Name, Email, Description, IsTrial, Is Active and Set Active.
	/// </summary>
	[System.ComponentModel.Description("Entity for Companies. Each Company should have only one Account. It contains the Properties Name, Email, Description, IsTrial, Is Active and Set Active.")]
	public class Account : BaseEntity
	{
		// Properties

		/// <summary>
		/// Gets or sets the name of the Company.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		[Required]
		[StringLength(30)]
		[System.ComponentModel.Description("Name of the Company")]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets companies email.
		/// </summary>
		/// <value>
		/// The email.
		/// </value>
		[Required]
		[StringLength(30)]
		[System.ComponentModel.Description("Email from companies main contact.")]
		public string Email { get; set; }

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>
		/// The description.
		/// </value>
		[StringLength(255)]
		[System.ComponentModel.Description("Description of the Company. Can be used for any purpose.")]
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this account is trial.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is trial; otherwise, <c>false</c>.
		/// </value>
		[System.ComponentModel.Description("Set if its a trial account.")]
		public bool IsTrial { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is active.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
		/// </value>
		[System.ComponentModel.Description("Set if the account is active.")]
		public bool IsActive { get; set; }

		/// <summary>
		/// Gets or sets the set active.
		/// </summary>
		/// <value>
		/// The set active.
		/// </value>
		[System.ComponentModel.Description("Set if the account is active.")]
		[Required]
		public DateTime SetActive { get; set; }

		// Navigation Properties

		/// <summary>
		/// Gets or sets navigation to users.
		/// </summary>
		/// <value>
		/// The users.
		/// </value>
		public virtual ICollection<User> Users { get; set; }
	}
}
