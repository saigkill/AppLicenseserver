// <copyright file="User.cs" company="marcos software">
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

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppLicenseserver.Entity
{
	/// <summary>
	/// Useraccount. A Company can have multiple Users. It contains the Properties FirstName,
	/// LastName, UserName, Email, Description, IsAdminRole, Roles, IsActive, Password, DecryptedPassword
	/// and AccountId.
	/// </summary>
	[System.ComponentModel.Description("Useraccount. A Company can have multiple Users. It contains the Properties FirstName, LastName, UserName, Email, Description, IsAdminRole, Roles, IsActive, Password, DecryptedPassword and AccountId")]

	public class User : BaseEntity
	{
		// Properties

		/// <summary>
		/// Gets or sets the first name.
		/// </summary>
		/// <value>
		/// The first name.
		/// </value>
		[Required]
		[StringLength(20)]
		[System.ComponentModel.Description("Users first name")]
		public string FirstName { get; set; }

		/// <summary>
		/// Gets or sets the last name.
		/// </summary>
		/// <value>
		/// The last name.
		/// </value>
		[Required]
		[StringLength(20)]
		[System.ComponentModel.Description("Users last name")]
		public string LastName { get; set; }

		/// <summary>
		/// Gets or sets the name of the user.
		/// </summary>
		/// <value>
		/// The name of the user.
		/// </value>
		[StringLength(30)]
		[System.ComponentModel.Description("Users username. Currently unused")]
		public string UserName { get; set; }

		/// <summary>
		/// Gets or sets users email address.
		/// </summary>
		[Required]
		[StringLength(30)]
		[System.ComponentModel.Description("Users Emailaddress")]
		public string Email { get; set; }

		/// <summary>
		/// Gets or sets the description. Maybe the Department can be set there.
		/// </summary>
		[MaxLength(255)]
		[StringLength(255)]
		[System.ComponentModel.Description("Department can be set there")]
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is admin role.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is admin role; otherwise, <c>false</c>.
		/// </value>
		[System.ComponentModel.Description("If user has Admin role set it there")]
		public bool IsAdminRole { get; set; }

		/// <summary>
		/// Gets or sets the Roles from the user. If User is Administrator, write &quot;Administrator&quot;. Other Userroles aren&apos;t planned.
		/// yet.
		/// </summary>
		[StringLength(255)]
		[System.ComponentModel.Description("If User is Administrator, write \"Administrator\". Other Useraccounts aren't plannet yet.")]
		public string Roles { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is active.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
		/// </value>
		[System.ComponentModel.Description("If active set it there")]
		public bool IsActive { get; set; }

		/// <summary>
		/// Gets or sets users password. Will be stored encrypted.
		/// </summary>
		[Required]
		[StringLength(100)]
		[System.ComponentModel.Description("Will be stored encrypted")]
		public string Password { get; set; }

		/// <summary>
		/// Gets or sets the AccountId. (Foreign Key to Account).
		/// </summary>
		/// <value>
		/// The account identifier.
		/// </value>
		[ForeignKey("Account")]
		public int AccountId { get; set; }

		/// <summary>
		/// Gets or sets the account.
		/// </summary>
		/// <value>
		/// The account.
		/// </value>
		public virtual Account Account { get; set; }

		/// <summary>
		/// Gets or sets the decrypted password.
		/// </summary>
		/// <value>
		/// The decrypted password.
		/// </value>
		[NotMapped]
		public string DecryptedPassword
		{
			get { return EntityHelper.Decrypt(Password); }
			set { Password = EntityHelper.Encrypt(value); }
		}

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
