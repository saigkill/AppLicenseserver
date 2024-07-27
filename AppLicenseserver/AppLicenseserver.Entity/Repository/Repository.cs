// <copyright file="Repository.cs" company="marcos software">
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
using AppLicenseserver.Entity.UnitofWork;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace AppLicenseserver.Entity.Repository
{
	/// <summary>
	/// General repository class.
	/// </summary>
	/// <typeparam name="T">The T object.</typeparam>
	public class Repository<T> : IRepository<T>
		where T : class
	{
		private readonly IUnitOfWork _unitOfWork;

		/// <summary>
		/// Initializes a new instance of the <see cref="Repository{T}"/> class.
		/// </summary>
		/// <param name="unitOfWork">The unit of work.</param>
		public Repository(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		/// <summary>
		/// Gets all.
		/// </summary>
		/// <returns>
		/// A IEnumerable of all needed entities.
		/// </returns>
		public IEnumerable<T> GetAll()
		{
			return _unitOfWork.Context.Set<T>();
		}

		/// <summary>
		/// Gets the specified predicate.
		/// </summary>
		/// <param name="predicate">The predicate.</param>
		/// <returns>
		/// A IEnumerable of function.
		/// </returns>
		public IEnumerable<T> Get(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
		{
			return _unitOfWork.Context.Set<T>().Where(predicate).AsEnumerable<T>();
		}

		/// <summary>
		/// Gets the one.
		/// </summary>
		/// <param name="predicate">The predicate.</param>
		/// <returns>
		/// A IEnumerable of function.
		/// </returns>
		public T GetOne(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
		{
			return _unitOfWork.Context.Set<T>().Where(predicate).FirstOrDefault();
		}

		/// <summary>
		/// Inserts the specified entity.
		/// </summary>
		/// <param name="entity">The entity.</param>
		public void Insert(T entity)
		{
			if (entity != null)
			{
				_unitOfWork.Context.Set<T>().Add(entity);
			}
		}

		/// <summary>
		/// Updates the specified identifier.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="entity">The entity.</param>
		public void Update(object id, T entity)
		{
			if (entity != null)
			{
				// T entitytoUpdate = _unitOfWork.Context.Set<T>().Find(id);
				// if (entitytoUpdate != null)
				// 	_unitOfWork.Context.Entry(entitytoUpdate).CurrentValues.SetValues(entity);
				_unitOfWork.Context.Entry(entity).State = EntityState.Modified;
			}
		}

		/// <summary>
		/// Deletes the specified identifier.
		/// </summary>
		/// <param name="id">The identifier.</param>
		public void Delete(object id)
		{
			T entity = _unitOfWork.Context.Set<T>().Find(id);
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
		/// Reads the by stored procedure.
		/// </summary>
		/// <param name="sql">The SQL.</param>
		/// <param name="parameters">The parameters.</param>
		/// <returns>
		/// T Object.
		/// </returns>
		public IEnumerable<T> READbyStoredProcedure(string sql, SqlParameter[] parameters)
		{
			return _unitOfWork.Context.Set<T>().FromSqlRaw(sql, parameters).ToListAsync().GetAwaiter().GetResult();
		}

		/// <summary>
		/// Create, update, delete dby stored procedure.
		/// </summary>
		/// <param name="sql">The SQL.</param>
		/// <param name="parameters">The parameters.</param>
		/// <returns>
		/// Integer of Stored Procedure.
		/// </returns>
		public int CUDbyStoredProcedure(string sql, SqlParameter[] parameters)
		{
			return _unitOfWork.Context.Database.ExecuteSqlRaw(sql, parameters);
		}
	}
}
