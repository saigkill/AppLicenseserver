// <copyright file="IServiceAsync.cs" company="Sascha Manns">
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
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace AppLicenseserver.Domain.Service
{
    /// <summary>
    /// Interface for services.
    /// </summary>
    /// <typeparam name="Tv">The type of the v.</typeparam>
    /// <typeparam name="Te">The type of the e.</typeparam>
    public interface IServiceAsync<Tv, Te>
    {
        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Tv>> GetAll();

        /// <summary>
        /// Adds the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        Task<int> Add(Tv obj);

        /// <summary>
        /// Updates the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        Task<int> Update(Tv obj);

        /// <summary>
        /// Removes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        Task<int> Remove(int id);

        /// <summary>
        /// Gets the one.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        Task<Tv> GetOne(int id);

        /// <summary>
        /// Gets the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        Task<IEnumerable<Tv>> Get(Expression<Func<Te, bool>> predicate);
    }
}
