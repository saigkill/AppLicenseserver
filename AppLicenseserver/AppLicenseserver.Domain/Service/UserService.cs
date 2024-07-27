// <copyright file="UserService.cs" company="marcos software">
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
	/// The user service.
	/// </summary>
	/// <typeparam name="Tv">The type of the v.</typeparam>
	/// <typeparam name="Te">The type of the e.</typeparam>
	/// <seealso cref="AppLicenseserver.Domain.Service.GenericService&lt;Tv, Te&gt;" />
	public class UserService<Tv, Te> : GenericService<Tv, Te>
												where Tv : UserViewModel
												where Te : User
	{
		// DI must be implemented in specific service as well beside GenericService constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="UserService{Tv, Te}"/> class.
		/// </summary>
		/// <param name="unitOfWork">The unit of work.</param>
		/// <param name="mapper">The mapper.</param>
		public UserService(IUnitOfWork unitOfWork, IMapper mapper)
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

		// These service calls are examples of stored procedure use in Apincore REST API serice
		// READbyStoredProcedure(sql, parameters)
		// CUDbyStoredProcedure(sql, parameters).

		/// <summary>
		/// Gets the user by email.
		/// </summary>
		/// <param name="firstname">The firstname.</param>
		/// <param name="lastname">The lastname.</param>
		/// <returns>The user.</returns>
		public Tv GetOneUserByName(string firstname, string lastname)
		{
			var entity = _unitOfWork.GetRepository<Te>()
				.GetOne(predicate: x => x.FirstName == firstname && x.LastName == lastname);
			return _mapper.Map<Tv>(source: entity);
		}

		// stored procedure CREATE UPDATE DELETE
		// note:sp params must be in the same order like in sp

		/// <summary>
		/// Updates the email by username.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="email">The email.</param>
		/// <returns>Records (int) from getting user by username.</returns>
		public int UpdateEmailByUsername(string username, string email)
		{
			var parameters = new[]
			{
				new SqlParameter("@UserName", SqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = username },
				new SqlParameter("@Email", SqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = email },
			};
			string sql = "EXEC [dbo].[prUpdateEmailByUsername] @UserName, @Email";

			int records = _unitOfWork.GetRepository<User>().CUDbyStoredProcedure(sql, parameters);
			return records;
		}

		/// <summary>
		/// Removes some trash useraccount from tests.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <returns>Records (int) from deleted user.</returns>
		public int DeleteUserTrash(string username)
		{
			var parameters = new[]
			{
				new SqlParameter("@UserName", SqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = "username" },
			};

			string sql = "EXEC [dbo].[prDeleteUserTrash] @UserName";

			int records = _unitOfWork.GetRepository<User>().CUDbyStoredProcedure(sql, parameters);
			return records;
		}
	}
}
