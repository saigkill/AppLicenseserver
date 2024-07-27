// <copyright file="IServiceAsync.cs" company="marcos software">
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
