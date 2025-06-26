// <copyright file="UserAsyncController.cs" company="Sascha Manns">
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
using System.Threading.Tasks;

using AppLicenseserver.Domain;
using AppLicenseserver.Domain.Service;
using AppLicenseserver.Entity;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Serilog;

namespace AppLicenseserver.Api.Controllers
{
    /// <summary>
    /// Async Controller for user information.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    // [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class UserAsyncController : ControllerBase
    {
        private readonly UserServiceAsync<UserViewModel, User> _userServiceAsync;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAsyncController"/> class.
        /// </summary>
        /// <param name="userServiceAsync">The user service asynchronous.</param>
        public UserAsyncController(UserServiceAsync<UserViewModel, User> userServiceAsync)
        {
            _userServiceAsync = userServiceAsync;
        }

        #region Get

        /// <summary>
        /// Gets all users.
        /// </summary>
        /// <returns>Useritems</returns>
        [Authorize(Roles = "Administrator")]
        [HttpGet("getall")]
        //[Attributes.DDosAttackProtected]
        public async Task<IEnumerable<UserViewModel>> GetAll()
        {
            var items = await _userServiceAsync.GetAll();
            return items;
        }

        /// <summary>
        /// Gets the user by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>OK or NotFound</returns>
        [Authorize(Roles = "Administrator")]
        [HttpGet("get/byid/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _userServiceAsync.GetOne(id);
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
        public async Task<IActionResult> GetActiveByLastName(string lastname)
        {
            var items = await _userServiceAsync.Get(a => a.IsActive && a.LastName == lastname);
            if (items == null)
            {
                Log.Error("GetActiveByLastName");
                return NotFound("The user record couldn't be found with lastname: " + lastname);
            }

            return Ok(items);
        }

        /// <summary>
        /// Gets active users by username.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>OK</returns>
        [Authorize(Roles = "Administrator")]
        [HttpGet("getactive/byusername/{username}")]
        public async Task<IActionResult> GetActiveByUserName(string username)
        {
            var items = await _userServiceAsync.Get(a => a.IsActive && a.UserName == username);
            if (items == null)
            {
                Log.Error("GetActiveByUsername({ ID}) NOT FOUND", username);
                return NotFound("The user record couldn't be found with username: " + username);
            }

            return Ok(items);
        }

        /// <summary>
        /// Gets the users by firstname and lastname. It uses READ stored procedure.
        /// </summary>
        /// <param name="firstname">The firstname.</param>
        /// <param name="lastname">The lastname.</param>
        /// <returns>Ok</returns>
        [Authorize(Roles = "Administrator")]
        [HttpGet("get/byname/{firstname}/{lastname}")]
        public async Task<IActionResult> GetUsersByName(string firstname, string lastname)
        {
            var items = await _userServiceAsync.GetUsersByName(firstname, lastname);
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
        public async Task<IActionResult> Create([FromBody] UserViewModel user)
        {
            if (user == null)
            {
                return BadRequest("UserViewModel is null. A valid UserViewModel contains FirstName, LastName, UserName, Email, Description, IsAdminRole, IsActive, Password and AccountId");
            }

            var id = await _userServiceAsync.Add(user);
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
        public async Task<IActionResult> Update(int id, [FromBody] UserViewModel user)
        {
            if (user == null || user.Id != id)
            {
                return BadRequest("UserViewModel is null or id is not equal to user.Id");
            }

            int retVal = await _userServiceAsync.Update(user);
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
        /// Updates the email by username.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="email">The email.</param>
        /// <returns>OK.</returns>
        [Authorize(Roles = "Administrator")]
        [HttpPut("update/byusernameemail/{username}/{email}")]
        public async Task<IActionResult> UpdateEmailbyUsername(string username, string email)
        {
            int id = await _userServiceAsync.UpdateEmailByUsername(username, email);
            return Ok(id);
        }
        #endregion
        #region Delete

        /// <summary>
        /// Deletes the specified user by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>NotFound, StatusCode 412 or NoContent</returns>
        [Authorize(Roles = "Administrator")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            int retVal = await _userServiceAsync.Remove(id);
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
        /// <returns>OK.</returns>
        [HttpDelete("deletetest/{username}")]
        [Authorize(Roles = "Administrator")]
        public IActionResult DeleteTest(string username)
        {
            var items = _userServiceAsync.DeleteUserTrash(username);
            return Ok(items);
        }
        #endregion
    }
}
