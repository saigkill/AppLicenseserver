// <copyright file="TelemetryAsyncController.cs" company="marcos software">
//   this file may not be redistributed in whole or significant part
//   and is subject to the marcos software license.
//
//   @author: Sascha Manns, s.manns@marcossoftware.com
//   @copyright: 2022, marcos-software.de, http://www.marcos-software.de
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
	/// Async Controller for telemetry data.
	/// </summary>
	/// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
	[ApiVersion("1.0")]
	[Route("api/[controller]")]
	// [Route("api/v{version:apiVersion}/[controller]")]
	[ApiController]
	public class TelemetryAsyncController : ControllerBase
	{
		private readonly TelemetryServiceAsync<TelemetryViewModel, Telemetry> _telemetryServiceAsync;

		/// <summary>
		/// Initializes a new instance of the <see cref="TelemetryAsyncController"/> class.
		/// </summary>
		/// <param name="telemetryServiceAsync">The telemetry service asynchronous.</param>
		public TelemetryAsyncController(TelemetryServiceAsync<TelemetryViewModel, Telemetry> telemetryServiceAsync)
		{
			_telemetryServiceAsync = telemetryServiceAsync;
		}

		#region Get

		/// <summary>
		/// Gets all telemetry data.
		/// </summary>
		/// <returns>Telemetry items</returns>
		[Authorize(Roles = "Administrator")]
		[HttpGet("getall")]
		public async Task<IActionResult> GetAll()
		{
			var items = await _telemetryServiceAsync.GetAll();
			return Ok(items);
		}

		/// <summary>
		/// Gets a special telemetry by identifier.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <returns>OK or NotFound Statuscode</returns>
		[Authorize(Roles = "Administrator")]
		[HttpGet("get/byid/{id}")]
		public async Task<IActionResult> GetById(int id)
		{
			var item = await _telemetryServiceAsync.GetOne(id);
			if (item == null)
			{
				Log.Error("GetById({ ID}) NOT FOUND", id);
				return NotFound("Telemetry item not found for id " + id);
			}

			return Ok(item);
		}

		/// <summary>
		/// Gets the telemetry by ip.
		/// </summary>
		/// <param name="ip">The ip.</param>
		/// <returns>Ok or NotFound</returns>
		[Authorize(Roles = "Administrator")]
		[HttpGet("get/byip/{ip}")]
		public async Task<IActionResult> GetByIp(string ip)
		{
			var item = await _telemetryServiceAsync.Get(a => a.Ip == ip);
			if (item == null)
			{
				Log.Error("GetByIp({ IP}) NOT FOUND", ip);
				return NotFound("Telemetry item not found for ip " + ip);
			}

			return Ok(item);
		}

		/// <summary>
		/// Gets the tekenetry by license identifier.
		/// </summary>
		/// <param name="licenseId">The identifier.</param>
		/// <returns>NotFound or OK</returns>
		[Authorize(Roles = "Administrator")]
		[HttpGet("get/bylicenseid/{licenseId}")]
		public async Task<IActionResult> GetByLicenseId(int licenseId)
		{
			var item = await _telemetryServiceAsync.Get(a => a.LicenseId == licenseId);
			if (item == null)
			{
				Log.Error("GetByLicenseId({ ID}) NOT FOUND", licenseId);
				return NotFound("Telemetry item not found for license id " + licenseId);
			}

			return Ok(item);
		}
		#endregion

		#region Post
		/// <summary>
		/// Creates the specified telemetry.
		/// </summary>
		/// <param name="telemetry">The telemetry.</param>
		/// <returns>BadRequest or Created.</returns>
		[Authorize(Roles = "Administrator")]
		[HttpPost("create/newtelemetry")]
		public async Task<IActionResult> Create([FromBody] TelemetryViewModel telemetry)
		{
			if (telemetry == null)
			{
				return BadRequest("Telemetry is null. You need Ip, ProductId, LicenseId, UserId to fulfill your Request.");
			}

			var id = await _telemetryServiceAsync.Add(telemetry);
			return Created($"api/Telemetry/{id}", id); // HTTP201 Resource created
		}
		#endregion

		#region Put

		/// <summary>
		/// Updates the specified telemetry by identifier.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="telemetry">The telemetry.</param>
		/// <returns>BadRequest, Accepted, Statuscode304/412</returns>
		[Authorize(Roles = "Administrator")]
		[HttpPut("update/{id}")]
		public async Task<IActionResult> Update(int id, [FromBody] TelemetryViewModel telemetry)
		{
			if (telemetry == null || telemetry.Id != id)
			{
				return BadRequest("TelemetryViewModel is null or id is not equal to id in url. You need a TelemetryId to fulfill your Request.");
			}

			var retVal = await _telemetryServiceAsync.Update(telemetry);
			if (retVal == 0)
			{
				return StatusCode(304, "Nothing to do. No changes since last updates.");  // Not Modified
			}
			else if (retVal == -1)
			{
				return StatusCode(412, "DbUpdateConcurrencyException"); // 412 Precondition Failed  - concurrency
			}
			else
			{
				return Accepted(telemetry);
			}
		}
		#endregion

		#region Delete

		/// <summary>
		/// Deletes the specified telemetry by identifier.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <returns>NotFound, Statuscode 412 or NoContent</returns>
		[Authorize(Roles = "Administrator")]
		[HttpDelete("delete/{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var retVal = await _telemetryServiceAsync.Remove(id);
			if (retVal == 0)
			{
				return NotFound("No Telemetry found with id: " + id);  // Not Found 404
			}
			else if (retVal == -1)
			{
				return StatusCode(412, "DbUpdateConcurrencyException"); // Precondition Failed  - concurrency
			}
			else
			{
				return NoContent(); // No Content 204
			}
		}
		#endregion
	}
}
