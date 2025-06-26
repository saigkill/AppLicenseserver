// <copyright file="AccountController.cs" company="Sascha Manns">
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

//#pragma warning disable SA1309 // FieldNamesMustNotBeginWithUnderscore
//#pragma warning disable SA1515 // SingleLineCommentPreceedBlankLine

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
    /// Controller class for the account resource.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    // [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AccountService<AccountViewModel, Account> _accountService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        /// <param name="accountService">The account service.</param>
        public AccountController(AccountService<AccountViewModel, Account> accountService)
        {
            _accountService = accountService;
        }

        #region Get

        /// <summary>
        /// Gets all accounts.
        /// </summary>
        /// <returns>Ok Object</returns>
        [Authorize(Roles = "Administrator")]
        [HttpGet("getall")]
        public IEnumerable<AccountViewModel> GetAll()
        {
            var items = _accountService.GetAll();
            return items;
        }

        /// <summary>
        /// Gets one active account by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>NotFound or Ok object</returns>
        [Authorize(Roles = "Administrator")]
        [HttpGet("get/byid/{id}")]
        public IActionResult GetById(int id)
        {
            var item = _accountService.GetOne(id);
            if (item == null)
            {
                Log.Error("GetById({ ID}) NOT FOUND", id);
                return NotFound("The account couldn't be found for AccountId: " + id);
            }

            return Ok(item);
        }

        /// <summary>
        /// Gets the name of the by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>IActionResult with AccountViewModel</returns>
        [Authorize(Roles = "Administrator")]
        [HttpGet("get/byname/{name}")]
        public IActionResult GetByName(string name)
        {
            var item = _accountService.Get(a => a.Name == name);
            if (item == null)
            {
                Log.Error("GetByName({ Name}) NOT FOUND", name);
                return NotFound("The account couldn't be found for AccountName: " + name);
            }

            return Ok(item);
        }


        /// <summary>
        /// Gets all active accounts by name
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Ok Object</returns>
        [Authorize(Roles = "Administrator")]
        [HttpGet("get/activebyname/{name}")]
        public IActionResult GetActiveByName(string name)
        {
            var items = _accountService.Get(a => a.IsActive && a.Name == name);
            if (items == null)
            {
                Log.Error("GetActiveByName({ NAME}) NOT FOUND", name);
                return NotFound("The account couldn't be found for Name: " + name);
            }

            return Ok(items);
        }

        /// <summary>
        /// Gets the active accounts by email.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns>Found item</returns>
        [Authorize(Roles = "Administrator")]
        [HttpGet("get/activebyemail/{email}")]
        public IActionResult GetActiveByEmail(string email)
        {
            var item = _accountService.Get(a => a.IsActive && a.Email == email);
            if (item == null)
            {
                Log.Error("GetActiveByEmail({ Email}) NOT FOUND", email);
                return NotFound("The account couldn't be found for Email: " + email);
            }

            return Ok(item);
        }
        #endregion

        #region Post

        /// <summary> 
        /// Creates a new account.
        /// </summary>
        /// <param name="account">The account data from AccountViewModel</param>
        /// <returns>Created or bad request result object</returns>
        [Authorize(Roles = "Administrator")]
        [HttpPost("create/newaccount")]
        public IActionResult Create([FromBody] AccountViewModel account)
        {
            if (account == null)
            {
                return BadRequest("AccountViewModel is null. You need Name, Email, Description, IsTrial, IsActive, SetActive (Date when Set) and Users.");
            }

            var id = _accountService.Add(account);
            return Created($"api/Account/{id}", id);  // HTTP201 Resource created
        }
        #endregion

        #region Put
        /// <summary>
        /// Updates the specified account by id and AccountViewModel.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="account">The account object by AccountViewModel.</param>
        /// <returns>BadRequest, StatusCode 304/412 or Accepted</returns>
        [Authorize(Roles = "Administrator")]
        [HttpPut("update/{id}")]
        public IActionResult Update(int id, [FromBody] AccountViewModel account)
        {
            if (account == null || account.Id != id)
            {
                return BadRequest("No AccountID found. To fulfill your request it is needed, to know, what AccountID are affected.");
            }

            int retVal = _accountService.Update(account);
            if (retVal == 0)
            {
                return StatusCode(304, "Nothing to do. No changes done since last update.");  // Not Modified
            }
            else if (retVal == -1)
            {
                return StatusCode(412, "DbUpdateConcurrencyException");  // 412 Precondition Failed  - concurrency
            }
            else
            {
                return Accepted(account);
            }
        }
        #endregion

        #region Delete
        /// <summary>
        /// Deletes the specified account by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>NotFound, StatusCode 412 or NoContent objects</returns>
        [Authorize(Roles = "Administrator")]
        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            int retVal = _accountService.Remove(id);
            if (retVal == 0)
            {
                return NotFound("Nothing found to delete.");  // Not Found 404
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
        /// <param name="email">The emailaddress what should deleted.</param>
        /// <returns>OK.</returns>
        [HttpDelete("deletetest/{email}")]
        [Authorize]
        public IActionResult DeleteTest(string email)
        {
            var items = _accountService.DeleteAccountTrash(email);
            return Ok(items);
        }
        #endregion
    }
}
