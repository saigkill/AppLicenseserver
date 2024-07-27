// <copyright file="TelemetryController.cs" company="marcos software">
//   this file may not be redistributed in whole or significant part
//   and is subject to the marcos software license.
//
//   @author: Sascha Manns, s.manns@marcossoftware.com
//   @copyright: 2022, marcos-software.de, http://www.marcos-software.de
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
	/// Controller for telemetry data.
	/// </summary>
	/// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
	[ApiVersion("1.0")]
	[Route("api/[controller]")]
	// [Route("api/v{version:apiVersion}/[controller]")]
	[ApiController]
	public class TelemetryController : ControllerBase
	{
		private readonly TelemetryService<TelemetryViewModel, Telemetry> _telemetryService;

		/// <summary>
		/// Initializes a new instance of the <see cref="TelemetryController"/> class.
		/// </summary>
		/// <param name="telemetryService">The telemetry service.</param>
		public TelemetryController(TelemetryService<TelemetryViewModel, Telemetry> telemetryService)
		{
			_telemetryService = telemetryService;
		}

		#region Get

		/// <summary>
		/// Gets all telemetry data.
		/// </summary>
		/// <returns>Telemetry items</returns>
		[Authorize(Roles = "Administrator")]
		[HttpGet("getall")]
		public IEnumerable<TelemetryViewModel> GetAll()
		{
			// Serilog log examples 
			// Log.Information("Log: Log.Information");
			// Log.Warning("Log: Log.Warning");
			// Log.Error("Log: Log.Error");
			// Log.Fatal("Log: Log.Fatal");
			var items = _telemetryService.GetAll();
			return items;
		}

		/// <summary>
		/// Gets a special telemetry by identifier.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <returns>OK or NotFound Statuscode</returns>
		[Authorize(Roles = "Administrator")]
		[HttpGet("get/byid/{id}")]
		public IActionResult GetById(int id)
		{
			var item = _telemetryService.GetOne(id);
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
		/// <returns>NotFound or OK</returns>
		[Authorize(Roles = "Administrator")]
		[HttpGet("get/byip/{ip}")]
		public IActionResult GetByIp(string ip)
		{
			var item = _telemetryService.Get(a => a.Ip == ip);
			if (item == null)
			{
				Log.Error("GetByIp({ IP}) NOT FOUND", ip);
				return NotFound("Telemetry item not found for ip " + ip);
			}

			return Ok(item);
		}

		/// <summary>
		/// Gets the telemetry by license identifier.
		/// </summary>
		/// <param name="licenseId">The license identifier.</param>
		/// <returns>OK or NotFound</returns>
		[Authorize(Roles = "Administrator")]
		[HttpGet("get/bylicenseid/{licenseId}")]
		public IActionResult GetByLicenseId(int licenseId)
		{
			var item = _telemetryService.Get(a => a.LicenseId == licenseId);
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
		public IActionResult Create([FromBody] TelemetryViewModel telemetry)
		{
			if (telemetry == null)
			{
				return BadRequest("Telemetry is null. You need Ip, ProductId, LicenseId, UserId to fulfill your Request.");
			}

			var id = _telemetryService.Add(telemetry);
			return Created($"api/Telemetry/{id}", id);  // HTTP201 Resource created
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
		public IActionResult Update(int id, [FromBody] TelemetryViewModel telemetry)
		{
			if (telemetry == null || telemetry.Id != id)
			{
				return BadRequest("TelemetryViewModel is null or id is not equal to id in url. You need a TelemetryId to fulfill your Request.");
			}

			var retVal = _telemetryService.Update(telemetry);
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
		public IActionResult Delete(int id)
		{
			var retVal = _telemetryService.Remove(id);
			if (retVal == 0)
			{
				return NotFound("No Telemetry found with id: " + id);  // Not Found 404
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
		#endregion
	}
}
