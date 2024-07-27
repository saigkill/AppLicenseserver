// <copyright file="IService.cs" company="marcos software">
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

namespace AppLicenseserver.Domain.Service
{
	/// <summary>
	/// Interface for all services.
	/// </summary>
	/// <typeparam name="Tv">The type of the v.</typeparam>
	/// <typeparam name="Te">The type of the e.</typeparam>
	public interface IService<Tv, Te>
	{
		/// <summary>
		/// Gets all.
		/// </summary>
		/// <returns></returns>
		IEnumerable<Tv> GetAll();

		/// <summary>
		/// Adds the specified object.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <returns></returns>
		int Add(Tv obj);

		/// <summary>
		/// Updates the specified object.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <returns></returns>
		int Update(Tv obj);

		/// <summary>
		/// Removes the specified identifier.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <returns></returns>
		int Remove(int id);

		/// <summary>
		/// Gets the one.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <returns></returns>
		Tv GetOne(int id);

		/// <summary>
		/// Gets the specified predicate.
		/// </summary>
		/// <param name="predicate">The predicate.</param>
		/// <returns></returns>
		IEnumerable<Tv> Get(Expression<Func<Te, bool>> predicate);
	}
}
