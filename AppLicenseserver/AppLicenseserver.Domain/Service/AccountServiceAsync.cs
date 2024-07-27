// <copyright file="AccountServiceAsync.cs" company="marcos software">
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

using System.Data;
using AppLicenseserver.Entity;
using AppLicenseserver.Entity.UnitofWork;
using AutoMapper;
using Microsoft.Data.SqlClient;

namespace AppLicenseserver.Domain.Service
{
	/// <summary>
	/// AccountService (Async) class.
	/// </summary>
	/// <typeparam name="Tv">The type of the v.</typeparam>
	/// <typeparam name="Te">The type of the e.</typeparam>
	/// <seealso cref="AppLicenseserver.Domain.Service.GenericServiceAsync&lt;Tv, Te&gt;" />
	public class AccountServiceAsync<Tv, Te> : GenericServiceAsync<Tv, Te>
										where Tv : AccountViewModel
										where Te : Account
	{
		// DI must be implemented specific service as well beside GenericAsyncService constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="AccountServiceAsync{Tv, Te}"/> class.
		/// </summary>
		/// <param name="unitOfWork">The unit of work.</param>
		/// <param name="mapper">The mapper.</param>
		public AccountServiceAsync(IUnitOfWork unitOfWork, IMapper mapper)
		{
			if (_unitOfWork == null)
			{
				_unitOfWork = unitOfWork;
			}

			if (_mapper == null)
			{
				_mapper = mapper;
			}
		}

		/// <summary>
		/// Removes some trash account from tests.
		/// </summary>
		/// <param name="email">The email.</param>
		/// <returns>Records what matches the email.</returns>
		public int DeleteAccountTrash(string email)
		{
			var parameters = new[]
			{
				new SqlParameter("@Email", SqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = "email" },
			};

			string sql = "EXEC [dbo].[prDeleteAccountTrash] @Email";

			int records = _unitOfWork.GetRepository<User>().CUDbyStoredProcedure(sql, parameters);
			return records;
		}
	}
}
