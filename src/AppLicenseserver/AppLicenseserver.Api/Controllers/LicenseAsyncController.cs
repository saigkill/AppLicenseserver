// <copyright file="LicenseAsyncController.cs" company="Sascha Manns">
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

using AppLicenseserver.Domain;
using AppLicenseserver.Domain.Service;
using AppLicenseserver.Entity;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Serilog;

namespace AppLicenseserver.Api.Controllers
{
    /// <summary>
    /// Async Controller for the license management
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    // [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class LicenseAsyncController : ControllerBase
    {
        private readonly LicenseServiceAsync<LicenseViewModel, License> _licenseServiceAsync;

        /// <summary>
        /// Initializes a new instance of the <see cref="LicenseAsyncController"/> class.
        /// </summary>
        /// <param name="licenseServiceAsync">The license service asynchronous.</param>
        public LicenseAsyncController(LicenseServiceAsync<LicenseViewModel, License> licenseServiceAsync)
        {
            _licenseServiceAsync = licenseServiceAsync;
        }

        #region Get
        /// <summary>
        /// Gets all licenses.
        /// </summary>
        /// <returns>Found license items.</returns>
        [Authorize(Roles = "Administrator")]
        [HttpGet("getall")]
        public async Task<IActionResult> GetAll()
        {
            var items = await _licenseServiceAsync.GetAll();
            return Ok(items);
        }

        /// <summary>
        /// Gets a license the by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>NotFound or OK Statuscodes</returns>
        [Authorize(Roles = "Administrator")]
        [HttpGet("get/byid/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _licenseServiceAsync.GetOne(id);
            if (item == null)
            {
                Log.Error("GetById({ ID}) NOT FOUND", id);
                return NotFound("No license found with the LicenseId " + id);
            }

            return Ok(item);
        }

        /// <summary>
        /// Gets the license by user identifier.
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <returns>NotFound or OK</returns>
        [Authorize(Roles = "Administrator")]
        [HttpGet("get/byuserid/{userid}")]
        public async Task<IActionResult> GetByUserId(int userid)
        {
            var item = await _licenseServiceAsync.Get(a => a.UserId == userid);
            if (item == null)
            {
                Log.Error("GetByUserId({ USERID}) NOT FOUND", userid);
                return NotFound("No licenses found for the UserId " + userid);
            }

            return Ok(item);
        }

        /// <summary>
        /// Gets license by licensenumber.
        /// </summary>
        /// <param name="licensenumber">The licensenumber.</param>
        /// <returns>NotFound or OK</returns>
        [Authorize]
        [HttpGet("get/bylicensenumber/{licensenumber}")]
        public async Task<IActionResult> GetByLicensenumber(string licensenumber)
        {
            var item = await _licenseServiceAsync.GetOne(licensenumber);
            if (item == null)
            {
                Log.Error("GetByLicensenumber({ LICENSENUMBER}) NOT FOUND", licensenumber);
                return NotFound("No license found with the number " + licensenumber);
            }

            return Ok(item);
        }
        #endregion

        #region Post
        /// <summary>
        /// Creates a new license.
        /// </summary>
        /// <param name="license">The license from LicenseViewModel.</param>
        /// <returns>BadRequest or Created Statuscode</returns>
        [Authorize(Roles = "Administrator")]
        [HttpPost("create/newalicense")]
        public async Task<IActionResult> Create([FromBody] LicenseViewModel license)
        {
            if (license == null)
            {
                Log.Error("Create() LicenseViewModel is NULL");
                return BadRequest("No valid license view model. You need to set the licensenumber, ProductId, Product, UserId and User to fullfill your request");
            }

            var id = await _licenseServiceAsync.Add(license);
            Log.Information("Create() LicenseViewModel with ID { ID} created", id);
            return Created($"api/License/{id}", id);  // HTTP201 Resource created
        }
        #endregion

        #region Put
        /// <summary>
        /// Updates the specified license by id.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="license">The license.</param>
        /// <returns>BadRequest, Accepted or Statuscode 304/412</returns>
        [Authorize(Roles = "Administrator")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] LicenseViewModel license)
        {
            if (license == null || license.Id != id)
            {
                Log.Error("Update() LicenseViewModel is NULL or ID is not equal");
                return BadRequest("No valid LicenseId found. You need to set the ID to fullfill your request");
            }

            var retVal = await _licenseServiceAsync.Update(license);
            if (retVal == 0)
            {
                Log.Information("Update() The data for { ID} hasn't been changed since last update.", id);
                return StatusCode(304, "Nothing to do. The data hasn't changed since last update.");  // Not Modified
            }
            else if (retVal == -1)
            {
                Log.Error("Update() The data for { ID} couldn't be changed", id);
                return StatusCode(412, "The Precognition before writing to database failed. Its a internal error, so better ask the Dev-Team.");  // Precondition Failed
            }
            else
            {
                Log.Information("Update() The data for { ID} has been changed", id);
                return Accepted(license);
            }
        }
        #endregion

        #region Delete
        /// <summary>
        /// Deletes the specified license by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Not Found, NoContent or StatusCode 412</returns>
        [Authorize(Roles = "Administrator")]
        [HttpDelete("delete/byid/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var retVal = await _licenseServiceAsync.Remove(id);
            if (retVal == 0)
            {
                Log.Error("Delete() License with ID { ID } was not found", id);
                return NotFound("No license found with the Id " + id);
            }
            else if (retVal == -1)
            {
                Log.Error("Delete() License with ID { ID } couldn't be deleted because of DB Concurrency Problems", id);
                return StatusCode(412, "DbUpdateConcurrencyException");  // Precondition Failed  - concurrency
            }
            else
            {
                Log.Information("Delete() License with ID { ID } followed the white rabbit", id);
                return NoContent();          // No Content 204
            }
        }

        /// <summary>
        /// Removes the licensenumber.
        /// </summary>
        /// <param name="licensenumber">The licensenumber.</param>
        /// <returns>NotFound, StatusCode412 or NoContent.</returns>
        [Authorize(Roles = "Administrator")]
        [HttpDelete("delete/bylicensenumber/{licensenumber}")]
        public async Task<IActionResult> RemoveLicensenumber(string licensenumber)
        {
            var retval = await _licenseServiceAsync.RemoveLicensenumber(licensenumber);
            if (retval == 0)
            {
                Log.Error("RemoveLicensenumber() License with number { LICENSENUMBER } was not found", licensenumber);
                return NotFound("No license found with the number " + licensenumber);
            }
            else if (retval == -1)
            {
                Log.Error("RemoveLicensenumber() License with number { LICENSENUMBER } couldn't be deleted because of DB Concurrency Problems", licensenumber);
                return StatusCode(412, "DbUpdateConcurrencyException");  // Precondition Failed  - concurrency
            }
            else
            {
                Log.Information("RemoveLicensenumber() License with number { LICENSENUMBER } followed the white rabbit", licensenumber);
                return NoContent();
            }
        }
        #endregion
    }
}
