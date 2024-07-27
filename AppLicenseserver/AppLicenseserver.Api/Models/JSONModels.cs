// <copyright file="JSONModels.cs" company="marcos software">
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

namespace AppLicenseserver.Api.Models
{
	/// <summary>
	/// This Class Contains the JSON Models for the API.
	/// </summary>
	public class JSONModels
	{
		/// <summary>
		/// Account and User JSON Model.
		/// </summary>
		public class Account
		{
			/// <summary>
			/// The AccountJsonModel.
			/// </summary>
			public class AccountJsonModel
			{
				/// <summary>
				/// Gets or sets the name.
				/// </summary>
				/// <value>
				/// The name.
				/// </value>
				public string Name { get; set; }

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
				/// Gets or sets a value indicating whether this instance is trial.
				/// </summary>
				/// <value>
				///   <c>true</c> if this instance is trial; otherwise, <c>false</c>.
				/// </value>
				public bool IsTrial { get; set; }

				/// <summary>
				/// Gets or sets a value indicating whether this instance is active.
				/// </summary>
				/// <value>
				///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
				/// </value>
				public bool IsActive { get; set; }

				/// <summary>
				/// Gets or sets the set active.
				/// </summary>
				/// <value>
				/// The set active.
				/// </value>
				public DateTime SetActive { get; set; }

				/// <summary>
				/// Gets or sets the users.
				/// </summary>
				/// <value>
				/// The users.
				/// </value>
				public ICollection<UserJsonModel> Users { get; set; }

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
				/// Gets or sets the test text.
				/// </summary>
				/// <value>
				/// The test text.
				/// </value>
				[StringLength(50)]
				public string TestText { get; set; } // string item for T4 generated tests
			}

			/// <summary>
			/// The User Json Model.
			/// </summary>
			public class UserJsonModel
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
				public string Roles { get; set; }

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
				public string Password { get; set; } // stored encrypted

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
				/// Gets or sets the test text.
				/// </summary>
				/// <value>
				/// The test text.
				/// </value>
				[StringLength(50)]
				public string TestText { get; set; }  // string item for T4 generated tests
			}
		}
	}
}
