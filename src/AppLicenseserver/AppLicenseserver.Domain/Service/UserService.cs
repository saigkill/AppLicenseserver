// <copyright file="UserService.cs" company="Sascha Manns">
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
