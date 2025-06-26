// <copyright file="AccountAsyncController.cs" company="Sascha Manns">
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

using System.Threading.Tasks;

using AppLicenseserver.Api.Models;
using AppLicenseserver.Domain;
using AppLicenseserver.Domain.Service;
using AppLicenseserver.Entity;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Serilog;

namespace AppLicenseserver.Api.Controllers
{
    /// <summary>
    /// Async Controller class for the account resource.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    // [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AccountAsyncController : ControllerBase
    {
        private readonly AccountServiceAsync<AccountViewModel, Account> _accountServiceAsync;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountAsyncController"/> class.
        /// </summary>
        /// <param name="accountServiceAsync">The account service asynchronous.</param>
        public AccountAsyncController(AccountServiceAsync<AccountViewModel, Account> accountServiceAsync)
        {
            _accountServiceAsync = accountServiceAsync;
        }

        #region Get
        /// <summary>
        /// Gets all accounts.
        /// </summary>
        /// <returns>All found accounts.</returns>
        /// <response code="200">Returns all accounts.</response>
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(JSONModels.Account.AccountJsonModel), StatusCodes.Status200OK)]
        [Produces("application/json")]
        [HttpGet("getall")]
        public async Task<IActionResult> GetAll()
        {
            var items = await this._accountServiceAsync.GetAll();
            return Ok(items);
        }

        /// <summary>
        /// Gets one active account by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>A special account by id.</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/get/1
        ///
        /// </remarks>
        /// <response code="200">Returns the account.</response>
        /// <response code="404">If the account is not found.</response>
        [Authorize(Roles = "Administrator")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(JSONModels.Account.AccountJsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("get/byid/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _accountServiceAsync.GetOne(id);
            if (item == null)
            {
                Log.Error("GetById({ ID}) NOT FOUND", id);
                return NotFound("The account couldn't be found for AccountId: " + id);
            }

            return Ok(item);
        }

        /// <summary>
        /// Gets all active accounts by name
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The account by its name</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/account/name/Marcos Software
        ///
        /// </remarks>
        /// <response code="200">Returns the account</response>
        /// <response code="404">If the account is not found</response>
        [Authorize(Roles = "Administrator")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(JSONModels.Account.AccountJsonModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("get/activebyname/{name}")]
        public async Task<IActionResult> GetActiveByName(string name)
        {
            var items = await _accountServiceAsync.Get(a => a.IsActive && a.Name == name);
            if (items == null)
            {
                Log.Error("GetActiveByName({ NAME} NOT FOUND", name);
                return NotFound("The account couldn't be found for Name: " + name);
            }

            return Ok(items);
        }

        /// <summary>
        /// Gets the active account by email.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns>The account by its email</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///
        /// </remarks>
        [Authorize(Roles = "Administrator")]
        [HttpGet("get/activebyemail/{email}")]
        public async Task<IActionResult> GetActiveByEmail(string email)
        {
            var items = await _accountServiceAsync.Get(a => a.IsActive && a.Email == email);
            if (items == null)
            {
                Log.Error("GetActiveByEmail({ EMAIL} NOT FOUND", email);
                return NotFound("The account couldn't be found for Email: " + email);
            }

            return Ok(items);
        }
        #endregion

        #region Post
        /// <summary>
        /// Creates a new account
        /// </summary>
        /// <param name="account">The account data from AccountViewModel</param>
        /// <returns>Created or bad request result object</returns>
        [Authorize(Roles = "Administrator")]
        [HttpPost("create/newaccount")]
        public async Task<IActionResult> Create([FromBody] AccountViewModel account)
        {
            if (account == null)
            {
                return BadRequest("AccountViewModel is null. You need Name, Email, Description, IsTrial, IsActive, SetActive (Date when Set) and Users.");
            }

            var id = await _accountServiceAsync.Add(account);
            return Created($"api/Account/{id}", id);  // HTTP201 Resource created
        }
        #endregion

        #region Put
        /// <summary>
        /// Updates the specified account by id and AccountViewModel
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="account">The account object by AccountViewModel.</param>
        /// <returns>BadRequest, StatusCode 304/412 or Accepted</returns>
        [Authorize(Roles = "Administrator")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AccountViewModel account)
        {
            if (account == null || account.Id != id)
            {
                return BadRequest("No AccountID found. To fulfill your request it is needed, to know, what AccountID are affected.");
            }

            int retVal = await _accountServiceAsync.Update(account);
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
        /// Deletes the specified account by identifier
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>NotFound, StatusCode 412 or NoContent objects</returns>
        [Authorize(Roles = "Administrator")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            int retVal = await _accountServiceAsync.Remove(id);
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
        /// <param name="email">The emailadress that should deleted.</param>
        /// <returns>OK.</returns>
        [Authorize(Roles = "Administrator")]
        [HttpDelete("deletetest/{email}")]
        public IActionResult DeleteTest(string email)
        {
            var items = _accountServiceAsync.DeleteAccountTrash(email);
            return Ok(items);
        }

        #endregion
    }
}
