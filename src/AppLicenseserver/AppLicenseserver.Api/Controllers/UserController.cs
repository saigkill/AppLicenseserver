// <copyright file="UserController.cs" company="Sascha Manns">
// Copyright (c) 2025 Sascha Manns.
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
// associated documentation files (the “Software”), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial
// portions of the Software.
//
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
// PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN
// ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

#pragma warning disable SA1309 // FieldNamesMustNotBeginWithUnderscore
#pragma warning disable SA1515 // SingleLineCommentPreceedBlankLine

using System.Collections.Generic;

using AppLicenseserver.Domain;
using AppLicenseserver.Domain.Service;
using AppLicenseserver.Entity;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Serilog;

namespace AppLicenseserver.Api.Controllers
{
    /// <summary>
    /// Controller for user information.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    // [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService<UserViewModel, User> _userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="userService">The user service.</param>
        public UserController(UserService<UserViewModel, User> userService)
        {
            _userService = userService;
        }

        #region Get

        /// <summary>
        /// Gets all users.
        /// </summary>
        /// <returns>Useritems</returns>
        [Authorize(Roles = "Administrator")]
        [HttpGet("getall")]
        public IEnumerable<UserViewModel> GetAll()
        {
            var items = _userService.GetAll();
            return items;
        }

        /// <summary>
        /// Gets the user by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>OK or NotFound</returns>
        [Authorize(Roles = "Administrator")]
        [HttpGet("get/byid/{id}")]
        public IActionResult GetById(int id)
        {
            var item = _userService.GetOne(id);
            if (item == null)
            {
                Log.Error("GetById({ ID}) NOT FOUND", id);
                return NotFound("The user record couldn't be found with id: " + id);
            }

            return Ok(item);
        }

        /// <summary>
        /// Gets all active users by lastname
        /// </summary>
        /// <param name="lastname">The name.</param>
        /// <returns>OK with items</returns>
        [Authorize(Roles = "Administrator")]
        [HttpGet("getactive/bylastname/{lastname}")]
        public IActionResult GetActiveByLastName(string lastname)
        {
            var items = _userService.Get(a => a.IsActive && a.LastName == lastname);
            if (items == null)
            {
                Log.Error("GetActiveByLastName({ LASTNAME}) NOT FOUND", lastname);
                return NotFound("The user record couldn't be found with lastname: " + lastname);
            }

            return Ok(items);
        }

        /// <summary>
        /// Gets the last name of the by.
        /// </summary>
        /// <param name="lastname">The lastname.</param>
        /// <returns>records</returns>
        [Authorize(Roles = "Administrator")]
        [HttpGet("get/bylastname/{lastname}")]
        public IActionResult GetByLastName(string lastname)
        {
            var items = _userService.Get(a => a.LastName == lastname);
            if (items == null)
            {
                Log.Error("GetActiveByLastName({ LASTNAME}) NOT FOUND", lastname);
                return NotFound("The user record couldn't be found with lastname: " + lastname);
            }

            return Ok(items);
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet("get/byusername/{username}")]
        public IActionResult GetByUsername(string username)
        {
            var items = _userService.Get(a => a.UserName == username);
            if (items == null)
            {
                Log.Error("GetActiveByUsername({ USERNAME}) NOT FOUND", username);
                return NotFound("The user record couldn't be found with username: " + username);
            }

            return Ok(items);
        }

        /// <summary>
        /// Gets the active by username.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>OK</returns>
        [Authorize(Roles = "Administrator")]
        [HttpGet("getactive/byusername/{username}")]
        public IActionResult GetActiveByUsername(string username)
        {
            var items = _userService.Get(a => a.IsActive && a.UserName == username);
            if (items == null)
            {
                Log.Error("GetActiveByUsername({ USERNAME}) NOT FOUND", username);
                return NotFound("The user record couldn't be found with username: " + username);
            }

            return Ok(items);
        }

        /// <summary>
        /// Gets the users by firstname and lastname.
        /// </summary>
        /// <param name="firstname">The firstname.</param>
        /// <param name="lastname">The lastname.</param>
        /// <returns>Ok</returns>
        [Authorize(Roles = "Administrator")]
        [HttpGet("get/byname/{firstname}/{lastname}")]
        public IActionResult GetUsersByName(string firstname, string lastname)
        {
            var items = _userService.GetOneUserByName(firstname, lastname);
            if (items == null)
            {
                Log.Error("GetUsersByName({ FIRSTNAME}, { LASTNAME}) NOT FOUND", firstname, lastname);
                return NotFound("The user record couldn't be found with firstname: " + firstname + " and lastname: " + lastname);
            }

            return Ok(items);
        }
        #endregion
        #region Post

        /// <summary>
        /// Creates the user by UserViewModel.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>BadRequest or Created</returns>
        [Authorize(Roles = "Administrator")]
        [HttpPost("create/newuser")]
        public IActionResult Create([FromBody] UserViewModel user)
        {
            if (user == null)
            {
                return BadRequest("UserViewModel is null. A valid UserViewModel contains FirstName, LastName, UserName, Email, Description, IsAdminRole, IsActive, Password and AccountId");
            }

            var id = _userService.Add(user);
            return Created($"api/User/{id}", id);  // HTTP201 Resource created
        }
        #endregion
        #region Put

        /// <summary>
        /// Updates the specified user by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="user">The user.</param>
        /// <returns>BadRequest, Statuscode 304/412 or Accepted</returns>
        [Authorize(Roles = "Administrator")]
        [HttpPut("update/byid/{id}")]
        public IActionResult Update(int id, [FromBody] UserViewModel user)
        {
            if (user == null || user.Id != id)
            {
                return BadRequest("UserViewModel is null or id is not equal to user.Id");
            }

            int retVal = _userService.Update(user);
            if (retVal == 0)
            {
                return StatusCode(304, "Nothing to do. No changes since last updates.");  // Not Modified
            }
            else if (retVal == -1)
            {
                return StatusCode(412, "DbUpdateConcurrencyException");  // 412 Precondition Failed  - concurrency
            }
            else
            {
                return Accepted(user);
            }
        }

        /// <summary>
        /// Updates the emailby username.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="email">The email.</param>
        /// <returns>OK</returns>
        [Authorize(Roles = "Administrator")]
        [HttpPut("update/byusernameemail/{username}/{email}")]
        public IActionResult UpdateEmailbyUsername(string username, string email)
        {
            // use CUD stored procedure example
            int id = _userService.UpdateEmailByUsername(username, email);
            return Ok(id);
        }
        #endregion
        #region Delete

        /// <summary>
        /// Deletes the specified user by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>NotFound, StatusCode 412 or NoContent</returns>
        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Delete(int id)
        {
            int retVal = _userService.Remove(id);
            if (retVal == 0)
            {
                return NotFound("No User found with Id: " + id);  // Not Found 404
            }
            else if (retVal == -1)
            {
                return StatusCode(412, "DbUpdateConcurrencyException");  // Precondition Failed  - concurrency
            }
            else
            {
                return NoContent();          // No Content 204
            }
        }

        /// <summary>
        /// Deletes old user accounts from test
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>OK</returns>
        [HttpDelete("deletetest/{username}")]
        [Authorize(Roles = "Administrator")]
        public IActionResult DeleteTrash(string username)
        {
            var items = _userService.DeleteUserTrash(username);
            return Ok(items);
        }
        #endregion
    }
}
