// <copyright file="Telemetry.cs" company="Sascha Manns">
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

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppLicenseserver.Entity
{
    /// <summary>
    /// Our Telemetry table. It contains Serialnumber, Ip-Number, ProductId and UserId.
    /// </summary>
    [System.ComponentModel.Description("Our Telemetry table. It contains Serialnumber, Ip-Number, ProductId and UserId.")]
    public class Telemetry : BaseEntity
    {
        // Properties

        /// <summary>
        /// Gets or sets customers Ip address.
        /// </summary>
        [Required]
        [MaxLength(16)]
        [StringLength(16)]
        [System.ComponentModel.Description("Users current IP Address")]
        public string Ip { get; set; }

        /// <summary>
        /// Gets or sets the ProductId (Foreign Key of the product).
        /// </summary>
        [Required]
        [ForeignKey("Product")]
        [System.ComponentModel.Description("Users current used product")]
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the product.
        /// </summary>
        /// <value>
        /// The product.
        /// </value>
        public virtual Product Product { get; set; }

        /// <summary>
        /// Gets or sets the LicenseId.
        /// </summary>
        [Required]
        [MaxLength(50)]
        [StringLength(50)]
        [ForeignKey("License")]
        [System.ComponentModel.Description("Users used License")]
        public int LicenseId { get; set; }

        /// <summary>
        /// Gets or sets the license.
        /// </summary>
        /// <value>
        /// The license.
        /// </value>
        public virtual License License { get; set; }

        /// <summary>
        /// Gets or sets the UserId (Foreign Key of the user).
        /// </summary>
        [Required]
        [ForeignKey("User")]
        [System.ComponentModel.Description("Users Id")]
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
