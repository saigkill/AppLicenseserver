// <copyright file="IRepository.cs" company="Sascha Manns">
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

using Microsoft.Data.SqlClient;

namespace AppLicenseserver.Entity.Repository
{
    /// <summary>
    /// Interface for Repository.
    /// </summary>
    /// <typeparam name="T">The T.</typeparam>
    public interface IRepository<T>
        where T : class
    {
        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns>A IEnumerable of all needed entities.</returns>
        IEnumerable<T> GetAll();

        /// <summary>
        /// Gets the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>A IEnumerable of function.</returns>
        IEnumerable<T> Get(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Gets the one.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>A IEnumerable of function.</returns>
        T GetOne(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Inserts the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Insert(T entity);

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Delete(T entity);

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        void Delete(object id);

        /// <summary>
        /// Updates the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="entity">The entity.</param>
        void Update(object id, T entity);

        /// <summary>
        /// Reads the by stored procedure.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>T Object.</returns>
        IEnumerable<T> READbyStoredProcedure(string sql, SqlParameter[] parameters);

        /// <summary>
        /// Create, update, delete dby stored procedure.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Integer of Stored Procedure.</returns>
        int CUDbyStoredProcedure(string sql, SqlParameter[] parameters);
    }
}
