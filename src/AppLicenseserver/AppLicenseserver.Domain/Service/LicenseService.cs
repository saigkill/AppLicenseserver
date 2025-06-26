// <copyright file="LicenseService.cs" company="Sascha Manns">
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
    /// The license service.
    /// </summary>
    /// <typeparam name="Tv">The type of the v.</typeparam>
    /// <typeparam name="Te">The type of the e.</typeparam>
    /// <seealso cref="AppLicenseserver.Domain.Service.GenericService&lt;Tv, Te&gt;" />
    public class LicenseService<Tv, Te> : GenericService<Tv, Te>
        where Tv : LicenseViewModel
        where Te : License
    {
        // DI must be implemented in specific service as well beside GenericService constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LicenseService{Tv, Te}"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="mapper">The mapper.</param>
        public LicenseService(IUnitOfWork unitOfWork, IMapper mapper)
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
        /// These service calls are examples of stored procedure use in Apincore REAT API serice
        /// READbyStoredProcedure(sql, parameters)
        /// CUDbyStoredProcedure(sql, parameters).
        /// </summary>
        /// <param name="licensenumber">The licensenumber.</param>
        /// <returns>The record with the licensenumber.</returns>
        public Tv GetOne(string licensenumber)
        {
            var entity = _unitOfWork.GetRepository<Te>()
                .GetOne(predicate: x => x.Licensenumber.ToString() == licensenumber);
            return _mapper.Map<Tv>(source: entity);
        }

        /// <summary>
        /// Gets the one active.
        /// </summary>
        /// <param name="licensenumber">The licensenumber.</param>
        /// <returns>The record with the licensenumber.</returns>
        public Tv GetOneActive(string licensenumber)
        {
            var entity = _unitOfWork.GetRepository<Te>()
                .GetOne(predicate: x => x.Licensenumber.ToString() == licensenumber && x.IsActive == true);
            return _mapper.Map<Tv>(source: entity);
        }

        /// <summary>
        /// Removes a license by its licensenumber.
        /// </summary>
        /// <param name="licensenumber">The licensenumber.</param>
        /// <returns>Records (int) from removed licensenumbers.</returns>
        public int RemoveLicensenumber(string licensenumber)
        {
            var parameters = new[]
            {
                new SqlParameter("@Licensenumber", SqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = licensenumber },
            };

            string sql = "EXEC [dbo].[prDeleteLicensenumber] @Licensenumber";

            int records = _unitOfWork.GetRepository<License>().CUDbyStoredProcedure(sql, parameters);
            return records;
        }
    }
}
