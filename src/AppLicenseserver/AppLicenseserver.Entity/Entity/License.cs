// <copyright file="License.cs" company="Sascha Manns">
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

#pragma warning disable SA1027 // TabsMustNotBeUsed
#pragma warning disable SA1200 // UsingDirectivesMustBePlacedWithinNamespace
#pragma warning disable SA1309 // FieldNamesMustNotBeginWithUnderscore
#pragma warning disable SA1101 // PrefixLocalCallsWithThis

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppLicenseserver.Entity
{
    /// <summary>
    /// Entity for Serials. A user can have multiple Serials (one per product). It contains
    /// the Properties Serialnumber, UserId, ProductId.
    /// </summary>
    [System.ComponentModel.Description("Entity for Serials. A user can have multiple Serials (one per product). It contains the Properties Licensenumber, UserId, ProductId.")]
    public class License : BaseEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="License"/> class.
        /// </summary>
        public License()
        {
            // Licensenumber = Guid.NewGuid();
        }

        // Properties

        /// <summary>
        /// Gets or sets the licensenumber.
        /// </summary>
        /// <value>
        /// The licensenumber.
        /// </value>
        [Required]
        [MaxLength(50)]
        [StringLength(50)]
        [Column(TypeName = "varchar(50)")]
        [System.ComponentModel.Description("Licensenumber will be generated automatically")]
        public Guid Licensenumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
        /// </value>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the product identifier.
        /// </summary>
        /// <value>
        /// The product identifier.
        /// </value>
        [Required]
        [System.ComponentModel.Description("ProductId used by Product")]
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the product.
        /// </summary>
        /// <value>
        /// The product.
        /// </value>
        public virtual Product Product { get; set; }

        /// <summary>
        /// Gets or sets the UserId (the foreign key to the User entity).
        /// </summary>
        [Required]
        [ForeignKey("User")]
        [System.ComponentModel.Description("UserId used by User")]
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>
        /// The user.
        /// </value>
        public virtual User User { get; set; }
    }
}
