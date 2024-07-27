// <copyright file="IRepository.cs" company="marcos software">
//   this file may not be redistributed in whole or significant part
//   and is subject to the marcos software license.
//
//   @author: Sascha Manns, s.manns@marcossoftware.com
//   @copyright: 2022, marcos-software.de, http://www.marcos-software.de
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
