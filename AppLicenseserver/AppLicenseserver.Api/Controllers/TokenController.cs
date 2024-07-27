// <copyright file="TokenController.cs" company="marcos software">
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
#pragma warning disable SA1629 // DocumentationTextMustEndWithAPeriod

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using AppLicenseserver.Domain;
using AppLicenseserver.Domain.Service;
using AppLicenseserver.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace JWT.Controllers
{
	/// <summary>
	/// Controller for the token endpoint.
	/// </summary>
	/// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
	[ApiVersion("1.0")]
	[Route("api/[controller]")]
	//[Route("api/v{version:apiVersion}/[controller]")]
	[ApiController]
	public class TokenController : Controller
	{
		private readonly IService<UserViewModel, User> _userService;
		private IConfiguration _config;

		/// <summary>
		/// Initializes a new instance of the <see cref="TokenController"/> class.
		/// </summary>
		/// <param name="config">The configuration.</param>
		/// <param name="userService">The user service.</param>
		public TokenController(IConfiguration config, IService<UserViewModel, User> userService)
		{
			_config = config;
			_userService = userService;
		}

		/// <summary>
		/// Creates the specified login from LoginModel.
		/// </summary>
		/// <param name="login">The login.</param>
		/// <returns>Unauthorized or OK Statuscode</returns>
		[AllowAnonymous]
		[HttpPost]
		//[DDosAttackProtected]
		public IActionResult Create([FromBody] LoginModel login)
		{
			IActionResult response = Unauthorized();
			var user = Authenticate(login);

			if (user != null)
			{
				var tokenString = BuildToken(user);
				response = Ok(new { token = tokenString });
			}

			return response;
		}

		/// <summary>
		/// Builds the token.
		/// </summary>
		/// <param name="user">The user.</param>
		/// <returns>New token</returns>
		private string BuildToken(UserModel user)
		{
			List<Claim> claims = new List<Claim>();
			claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Name));
			claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
			claims.Add(new Claim(JwtRegisteredClaimNames.Birthdate, user.Birthdate.ToString("yyyy-MM-dd")));
			claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

			// attach roles
			foreach (string role in user.Roles)
			{
				claims.Add(new Claim(ClaimTypes.Role, role));
			}

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
			   _config["Jwt:Issuer"],
			   _config["Jwt:Issuer"],
			   claims,
			   expires: DateTime.Now.AddDays(1),  // 1 day expiry and a client monitor token quality and should request new token with this one expiries
			   signingCredentials: creds);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		/// <summary>
		/// Authenticates login information, retrieves authorization infomation (roles).
		/// </summary>
		/// <param name="login">The login.</param>
		/// <returns>Userinformation as Array</returns>
		private UserModel Authenticate(LoginModel login)
		{
			UserModel user = null;

			var userView = _userService.Get(x => x.UserName == login.Username).SingleOrDefault();
			if (userView != null && userView.Password == login.Password)
			{
				user = new UserModel { Name = userView.FirstName + " " + userView.LastName, Email = userView.Email, Roles = new string[] { } };
				foreach (string role in userView.Roles)
				{
					user.Roles = new List<string>(user.Roles) { role }.ToArray();
				}

				if (userView.IsAdminRole)
				{
					user.Roles = new List<string>(user.Roles) { "Administrator" }.ToArray();
				}
			}

			return user;
		}

		/// <summary>
		/// Login Model class
		/// </summary>
		public class LoginModel
		{
			/// <summary>
			/// Gets or sets the username.
			/// </summary>
			/// <value>
			/// The username.
			/// </value>
			public string? Username { get; set; }

			/// <summary>
			/// Gets or sets the password.
			/// </summary>
			/// <value>
			/// The password.
			/// </value>
			public string? Password { get; set; }
		}

		/// <summary>
		/// User Model class
		/// </summary>
		private class UserModel
		{
			/// <summary>
			/// Gets or sets the name.
			/// </summary>
			/// <value>
			/// The name.
			/// </value>
			public string? Name { get; set; }

			/// <summary>
			/// Gets or sets the email.
			/// </summary>
			/// <value>
			/// The email.
			/// </value>
			public string? Email { get; set; }

			/// <summary>
			/// Gets or sets the birthdate.
			/// </summary>
			/// <value>
			/// The birthdate.
			/// </value>
			public DateTime Birthdate { get; set; }

			/// <summary>
			/// Gets or sets the roles.
			/// </summary>
			/// <value>
			/// The roles.
			/// </value>
			public string[] Roles { get; set; }
		}
	}
}