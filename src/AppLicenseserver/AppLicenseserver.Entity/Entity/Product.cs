// <copyright file="Product.cs" company="Sascha Manns">
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
using System.Collections.Generic;

namespace AppLicenseserver.Entity
{
    /// <summary>
    /// Our Product table. It contains all apps, what are released.
    /// </summary>
    [System.ComponentModel.Description("Our Product table. It contains all apps, what are released.")]
    public class Product : BaseEntity
    {
        // Properties

        /// <summary>
        /// Gets or sets the Name of the product.
        /// </summary>
        [System.ComponentModel.Description("Name of the product.")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets Description of the product.
        /// </summary>
        [System.ComponentModel.Description("Description of the product.")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the Version of the product.
        /// </summary>
        [System.ComponentModel.Description("Version of the product.")]
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the Release date of the product.
        /// </summary>
        [System.ComponentModel.Description("Release date of the product.")]
        public DateTime ReleaseDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product is released or not.
        /// </summary>
        [System.ComponentModel.Description("The product is released or not.")]
        public bool IsReleased { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product is active or not.
        /// </summary>
        [System.ComponentModel.Description("The product is active or not.")]
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product is deleted or not.
        /// </summary>
        [System.ComponentModel.Description("The product is deleted or not.")]
        public bool IsDeleted { get; set; }

        // Navigation Properties

        /// <summary>
        /// Gets or sets the licenses.
        /// </summary>
        /// <value>
        /// The licenses.
        /// </value>
        public virtual ICollection<License> Licenses { get; set; }

        /// <summary>
        /// Gets or sets the telemetries.
        /// </summary>
        /// <value>
        /// The telemetries.
        /// </value>
        public virtual ICollection<Telemetry> Telemetries { get; set; }
    }
}
