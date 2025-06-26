// <copyright file="GenericService.cs" company="Sascha Manns">
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

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using AppLicenseserver.Entity;
using AppLicenseserver.Entity.UnitofWork;

using AutoMapper;

namespace AppLicenseserver.Domain.Service
{
    /// <summary>
    /// Main class for Generic Service.
    /// </summary>
    /// <typeparam name="Tv">The type of the v.</typeparam>
    /// <typeparam name="Te">The type of the e.</typeparam>
    /// <seealso cref="AppLicenseserver.Domain.Service.IService&lt;Tv, Te&gt;" />
    public class GenericService<Tv, Te> : IService<Tv, Te>
        where Tv : BaseDomain
        where Te : BaseEntity
    {
        /// <summary>
        /// The unit of work.
        /// </summary>
        protected IUnitOfWork _unitOfWork;

        /// <summary>
        /// The mapper.
        /// </summary>
        protected IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericService{Tv, Te}"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="mapper">The mapper.</param>
        public GenericService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericService{Tv, Te}"/> class.
        /// </summary>
        public GenericService()
        {
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns>Tv as IEnumerable.</returns>
        public virtual IEnumerable<Tv> GetAll()
        {
            var entities = _unitOfWork.GetRepository<Te>()
            .GetAll();
            return _mapper.Map<IEnumerable<Tv>>(source: entities);
        }

        /// <summary>
        /// Gets the one.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>TV object.</returns>
        public virtual Tv GetOne(int id)
        {
            var entity = _unitOfWork.GetRepository<Te>()
                .GetOne(predicate: x => x.Id == id);
            return _mapper.Map<Tv>(source: entity);
        }

        /// <summary>
        /// Adds the specified view.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns>The entity id.</returns>
        public virtual int Add(Tv view)
        {
            var entity = _mapper.Map<Te>(source: view);
            _unitOfWork.GetRepository<Te>().Insert(entity);
            _unitOfWork.Save();
            return entity.Id;
        }

        /// <summary>
        /// Updates the specified view.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns>Result (integer) of saving _unitOfWork.Save.</returns>
        public virtual int Update(Tv view)
        {
            _unitOfWork.GetRepository<Te>().Update(view.Id, _mapper.Map<Te>(source: view));
            return _unitOfWork.Save();
        }

        /// <summary>
        /// Removes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Result (integer) of saving _unitOfWork.Save.</returns>
        public virtual int Remove(int id)
        {
            Te entity = _unitOfWork.Context.Set<Te>().Find(id);
            _unitOfWork.GetRepository<Te>().Delete(entity);
            return _unitOfWork.Save();
        }

        /// <summary>
        /// Gets the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>TV object as IENumerable.</returns>
        public virtual IEnumerable<Tv> Get(Expression<Func<Te, bool>> predicate)
        {
            var entities = _unitOfWork.GetRepository<Te>()
                .Get(predicate: predicate);
            return _mapper.Map<IEnumerable<Tv>>(source: entities);
        }
    }
}
