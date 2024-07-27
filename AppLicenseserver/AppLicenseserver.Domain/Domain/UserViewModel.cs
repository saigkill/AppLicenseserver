// <copyright file="UserViewModel.cs" company="marcos software">
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
using Newtonsoft.Json;

namespace AppLicenseserver.Domain
{
	/// <summary>
	/// A user attached to an account.
	/// </summary>
	public class UserViewModel : BaseDomain
	{
		/// <summary>
		/// Gets or sets the first name.
		/// </summary>
		/// <value>
		/// The first name.
		/// </value>
		public string FirstName { get; set; }

		/// <summary>
		/// Gets or sets the last name.
		/// </summary>
		/// <value>
		/// The last name.
		/// </value>
		public string LastName { get; set; }

		/// <summary>
		/// Gets or sets the name of the user.
		/// </summary>
		/// <value>
		/// The name of the user.
		/// </value>
		public string UserName { get; set; }

		/// <summary>
		/// Gets or sets the email.
		/// </summary>
		/// <value>
		/// The email.
		/// </value>
		public string Email { get; set; }

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>
		/// The description.
		/// </value>
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is admin role.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is admin role; otherwise, <c>false</c>.
		/// </value>
		public bool IsAdminRole { get; set; }

		/// <summary>
		/// Gets or sets the roles.
		/// </summary>
		/// <value>
		/// The roles.
		/// </value>
		public ICollection<string> Roles { get; set; } // map from semicolumn delimited from Entity

		/// <summary>
		/// Gets or sets a value indicating whether this instance is active.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
		/// </value>
		public bool IsActive { get; set; }

		/// <summary>
		/// Gets or sets the password.
		/// </summary>
		/// <value>
		/// The password.
		/// </value>
		public string Password { get; set; }

		/// <summary>
		/// Gets or sets the account identifier.
		/// </summary>
		/// <value>
		/// The account identifier.
		/// </value>
		public int AccountId { get; set; }

		/// <summary>
		/// Gets or sets the account.
		/// </summary>
		/// <value>
		/// The account.
		/// </value>
		[JsonIgnore] // to avoid circular serialization
		public virtual AccountViewModel Account { get; set; }
	}
}
