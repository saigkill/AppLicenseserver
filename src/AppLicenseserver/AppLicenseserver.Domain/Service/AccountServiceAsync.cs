// <copyright file="AccountServiceAsync.cs" company="Sascha Manns">
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
