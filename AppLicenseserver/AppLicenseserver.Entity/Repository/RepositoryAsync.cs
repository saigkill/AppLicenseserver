// <copyright file="RepositoryAsync.cs" company="marcos software">
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
using System.Linq;
using System.Threading.Tasks;
using AppLicenseserver.Entity.UnitofWork;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace AppLicenseserver.Entity.Repository
{
	/// <summary>
	/// General repository class async.
	/// </summary>
	/// <typeparam name="T">The T object.</typeparam>
	public class RepositoryAsync<T> : IRepositoryAsync<T>
		where T : class
	{
		private readonly IUnitOfWork _unitOfWork;

		/// <summary>
		/// Initializes a new instance of the <see cref="RepositoryAsync{T}"/> class.
		/// </summary>
		/// <param name="unitOfWork">The unit of work.</param>
		public RepositoryAsync(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		/// <summary>
		/// Gets all.
		/// </summary>
		/// <returns>
		/// A IEnumerable of all needed entities.
		/// </returns>
		public async Task<IEnumerable<T>> GetAll()
		{
			return await _unitOfWork.Context.Set<T>().ToListAsync();
		}

		/// <summary>
		/// Gets the specified predicate.
		/// </summary>
		/// <param name="predicate">The predicate.</param>
		/// <returns>
		/// A IEnumerable of function.
		/// </returns>
		public async Task<IEnumerable<T>> Get(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
		{
			return await _unitOfWork.Context.Set<T>().Where(predicate).ToListAsync();
		}

		/// <summary>
		/// Gets the one.
		/// </summary>
		/// <param name="predicate">The predicate.</param>
		/// <returns>
		/// A IEnumerable of function.
		/// </returns>
		public async Task<T> GetOne(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
		{
			return await _unitOfWork.Context.Set<T>().Where(predicate).FirstOrDefaultAsync();
		}

		/// <summary>
		/// Inserts the specified entity.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <returns>Insert Task.</returns>
		public async Task Insert(T entity)
		{
			if (entity != null)
			{
				await _unitOfWork.Context.Set<T>().AddAsync(entity);
			}
		}

		/// <summary>
		/// Updates the specified identifier.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="entity">The entity.</param>
		/// <returns>Task for update.</returns>
		public async Task Update(object id, T entity)
		{
			if (entity != null)
			{
				// T entitytoUpdate = await _unitOfWork.Context.Set<T>().FindAsync(id);
				// if (entitytoUpdate != null)
				// 	_unitOfWork.Context.Entry(entitytoUpdate).CurrentValues.SetValues(entity);
				_unitOfWork.Context.Entry(entity).State = EntityState.Modified;
			}
		}

		/// <summary>
		/// Deletes the specified identifier.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <returns>Result of Delete task.</returns>
		public async Task Delete(object id)
		{
			T entity = await _unitOfWork.Context.Set<T>().FindAsync(id);
			Delete(entity);
		}

		/// <summary>
		/// Deletes the specified entity.
		/// </summary>
		/// <param name="entity">The entity.</param>
		public void Delete(T entity)
		{
			if (entity != null)
			{
				_unitOfWork.Context.Set<T>().Remove(entity);
			}
		}

		/// <summary>
		/// Reads by stored procedure.
		/// </summary>
		/// <param name="sql">The SQL.</param>
		/// <param name="parameters">The parameters.</param>
		/// <returns>
		/// List of T.
		/// </returns>
		public Task<List<T>> READbyStoredProcedure(string sql, SqlParameter[] parameters)
		{
			return _unitOfWork.Context.Set<T>().FromSqlRaw(sql, parameters).ToListAsync();
		}

		/// <summary>
		/// Cus the dby stored procedure.
		/// </summary>
		/// <param name="sql">The SQL.</param>
		/// <param name="parameters">The parameters.</param>
		/// <returns>
		/// Integer of Stored Procedure.
		/// </returns>
		public Task<int> CUDbyStoredProcedure(string sql, SqlParameter[] parameters)
		{
			return _unitOfWork.Context.Database.ExecuteSqlRawAsync(sql, parameters);
		}
	}
}
