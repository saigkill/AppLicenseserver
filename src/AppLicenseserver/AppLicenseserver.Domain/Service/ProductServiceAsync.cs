// <copyright file="ProductServiceAsync.cs" company="Sascha Manns">
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

using AppLicenseserver.Entity;
using AppLicenseserver.Entity.UnitofWork;

using AutoMapper;

namespace AppLicenseserver.Domain.Service
{
    /// <summary>
    /// The product service (async).
    /// </summary>
    /// <typeparam name="Tv">The type of the v.</typeparam>
    /// <typeparam name="Te">The type of the e.</typeparam>
    /// <seealso cref="AppLicenseserver.Domain.Service.GenericServiceAsync&lt;Tv, Te&gt;" />
    public class ProductServiceAsync<Tv, Te> : GenericServiceAsync<Tv, Te>
        where Tv : ProductViewModel
        where Te : Product
    {
        // DI must be implemented in specific service as well beside GenericService constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductServiceAsync{Tv, Te}"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="mapper">The mapper.</param>
        public ProductServiceAsync(IUnitOfWork unitOfWork, IMapper mapper)
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
    }
}
