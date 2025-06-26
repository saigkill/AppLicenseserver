// <copyright file="Account.cs" company="Sascha Manns">
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
using System.ComponentModel.DataAnnotations;

namespace AppLicenseserver.Entity
{
    /// <summary>
    /// Entity for Companies. Each Company should have only one Account. It contains the
    /// Properties Name, Email, Description, IsTrial, Is Active and Set Active.
    /// </summary>
    [System.ComponentModel.Description("Entity for Companies. Each Company should have only one Account. It contains the Properties Name, Email, Description, IsTrial, Is Active and Set Active.")]
    public class Account : BaseEntity
    {
        // Properties

        /// <summary>
        /// Gets or sets the name of the Company.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [Required]
        [StringLength(30)]
        [System.ComponentModel.Description("Name of the Company")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets companies email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        [Required]
        [StringLength(30)]
        [System.ComponentModel.Description("Email from companies main contact.")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [StringLength(255)]
        [System.ComponentModel.Description("Description of the Company. Can be used for any purpose.")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this account is trial.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is trial; otherwise, <c>false</c>.
        /// </value>
        [System.ComponentModel.Description("Set if its a trial account.")]
        public bool IsTrial { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
        /// </value>
        [System.ComponentModel.Description("Set if the account is active.")]
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the set active.
        /// </summary>
        /// <value>
        /// The set active.
        /// </value>
        [System.ComponentModel.Description("Set if the account is active.")]
        [Required]
        public DateTime SetActive { get; set; }

        // Navigation Properties

        /// <summary>
        /// Gets or sets navigation to users.
        /// </summary>
        /// <value>
        /// The users.
        /// </value>
        public virtual ICollection<User> Users { get; set; }
    }
}
