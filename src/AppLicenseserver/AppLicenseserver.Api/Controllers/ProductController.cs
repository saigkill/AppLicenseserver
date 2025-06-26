// <copyright file="ProductController.cs" company="Sascha Manns">
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
    /// Controller for the product entity.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    //[Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductService<ProductViewModel, Product> _productService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductController"/> class.
        /// </summary>
        /// <param name="productService">The product service.</param>
        public ProductController(ProductService<ProductViewModel, Product> productService)
        {
            _productService = productService;
        }

        #region Get
        /// <summary>
        /// Gets all products.
        /// </summary>
        /// <returns>All product items</returns>
        [Authorize]
        [HttpGet("getall")]
        public IEnumerable<ProductViewModel> GetAll()
        {
            // Serilog log examples
            // Log.Information("Log: Log.Information");
            // Log.Warning("Log: Log.Warning");
            // Log.Error("Log: Log.Error");
            // Log.Fatal("Log: Log.Fatal");
            var items = _productService.GetAll();
            return items;
        }

        /// <summary>
        /// Gets product the by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>OK or NotFound Statuscode</returns>
        [Authorize]
        [HttpGet("get/byid/{id}")]
        public IActionResult GetById(int id)
        {
            var item = _productService.GetOne(id);
            if (item == null)
            {
                Log.Error("GetById({ ID}) NOT FOUND", id);
                return NotFound("Product with the was not found: " + id);
            }

            return Ok(item);
        }

        /// <summary>
        /// Gets product by name
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>OK or NotFound</returns>
        [Authorize]
        [HttpGet("get/byname/{name}")]
        public IActionResult GetByName(string name)
        {
            var item = _productService.Get(a => a.Name == name);
            if (item == null)
            {
                Log.Error("GetByName({ Name}) NOT FOUND", name);
                return NotFound("No product found with the name: " + name);
            }

            return Ok(item);
        }
        #endregion

        #region Post
        /// <summary>
        /// Creates a new product.
        /// </summary>
        /// <param name="product">The product.</param>
        /// <returns>BadRequest or Created Statuscode</returns>
        [Authorize(Roles = "Administrator")]
        [HttpPost("create/newproduct")]
        public IActionResult Create([FromBody] ProductViewModel product)
        {
            if (product == null)
            {
                return BadRequest("ProductViewModel is null. You need Name, Description, Version, ReleaseDate, IsReleased, IsActive, IsDeleted to fulfill your request.");
            }

            var id = _productService.Add(product);
            return Created($"api/Product/{id}", id);  // HTTP201 Resource created
        }
        #endregion

        #region Put
        /// <summary>
        /// Updates the specified product by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="product">The product.</param>
        /// <returns>BadRequest, Accepted, Statuscode304/412</returns>
        [Authorize(Roles = "Administrator")]
        [HttpPut("update/{id}")]
        public IActionResult Update(int id, [FromBody] ProductViewModel product)
        {
            if (product == null || product.Id != id)
            {
                return BadRequest("ProductViewModel is null. You need Name, Description, Version, ReleaseDate, IsReleased, IsActive, IsDeleted to fulfill your request.");
            }

            var retVal = _productService.Update(product);
            if (retVal == 0)
            {
                return StatusCode(304, "Nothing to do. No changes since last update.");  // Not Modified
            }
            else if (retVal == -1)
            {
                return StatusCode(412, "DbUpdateConcurrencyException");  // 412 Precondition Failed  - concurrency
            }
            else
            {
                return Accepted(product);
            }
        }
        #endregion

        #region Delete
        /// <summary>
        /// Deletes the specified product by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>NotFound, NoContent or StatusCode 412</returns>
        [Authorize(Roles = "Administrator")]
        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            var retVal = _productService.Remove(id);
            if (retVal == 0)
            {
                return NotFound("Not found ProductId: " + id);  // Not Found 404
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
