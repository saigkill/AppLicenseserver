// <copyright file="AccountService.cs" company="marcos software">
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
	/// Account Service class.
	/// </summary>
	/// <typeparam name="Tv">The type of the v.</typeparam>
	/// <typeparam name="Te">The type of the e.</typeparam>
	/// <seealso cref="AppLicenseserver.Domain.Service.GenericService&lt;Tv, Te&gt;" />
	public class AccountService<Tv, Te> : GenericService<Tv, Te>
										where Tv : AccountViewModel
										where Te : Account
	{
		// DI must be implemented in specific service as well beside GenericService constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="AccountService{Tv, Te}"/> class.
		/// </summary>
		/// <param name="unitOfWork">The unit of work.</param>
		/// <param name="mapper">The mapper.</param>
		public AccountService(IUnitOfWork unitOfWork, IMapper mapper)
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

		// add here any custom service method or override generic service method

		/// <summary>
		/// Do Nothing demo method.
		/// </summary>
		/// <returns>true.</returns>
		public bool DoNothing()
		{
			return true;
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
