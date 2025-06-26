// <copyright file="UnitofWork.cs" company="Sascha Manns">
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
#pragma warning disable SA1649 // FileMustMatchFirstType

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AppLicenseserver.Entity.Context;
using AppLicenseserver.Entity.Repository;

using Microsoft.EntityFrameworkCore;


namespace AppLicenseserver.Entity.UnitofWork
{
    /// <summary>
    /// IUinitofWork derived class from IDisposable.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Gets the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>TEntity Object.</returns>
        IRepository<TEntity> GetRepository<TEntity>()
            where TEntity : class;

        /// <summary>
        /// Gets the repository asynchronous.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>TEntity object.</returns>
        IRepositoryAsync<TEntity> GetRepositoryAsync<TEntity>()
            where TEntity : class;

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        DefaultDbContext Context { get; }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        /// <returns>Integer result from Save().</returns>
        int Save();

        /// <summary>
        /// Saves the asynchronous.
        /// </summary>
        /// <returns>Integer result from SaveAsync().</returns>
        Task<int> SaveAsync();
    }

    /// <summary>
    /// Interface for TContext.
    /// </summary>
    /// <typeparam name="TContext">The type of the context.</typeparam>
    /// <seealso cref="System.IDisposable" />
    public interface IUnitOfWork<TContext> : IUnitOfWork
        where TContext : DbContext
    {
    }

    /// <summary>
    /// UnitToWork class based on interface below.
    /// </summary>
    /// <seealso cref="AppLicenseserver.Entity.UnitofWork.IUnitOfWork" />
    public class UnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        public DefaultDbContext Context { get; }

        private Dictionary<Type, object> _repositoriesAsync;
        private Dictionary<Type, object> _repositories;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public UnitOfWork(DefaultDbContext context)
        {
            Context = context;
            _disposed = false;
        }

        /// <summary>
        /// Gets the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>
        /// TEntity Object.
        /// </returns>
        public IRepository<TEntity> GetRepository<TEntity>()
            where TEntity : class
        {
            if (_repositories == null)
            {
                _repositories = new Dictionary<Type, object>();
            }

            var type = typeof(TEntity);
            if (!_repositories.ContainsKey(type))
            {
                _repositories[type] = new Repository<TEntity>(this);
            }

            return (IRepository<TEntity>)_repositories[type];
        }

        /// <summary>
        /// Gets the repository asynchronous.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>
        /// TEntity object.
        /// </returns>
        public IRepositoryAsync<TEntity> GetRepositoryAsync<TEntity>()
            where TEntity : class
        {
            if (_repositories == null)
            {
                _repositoriesAsync = new Dictionary<Type, object>();
            }

            var type = typeof(TEntity);
            if (!_repositoriesAsync.ContainsKey(type))
            {
                _repositoriesAsync[type] = new RepositoryAsync<TEntity>(this);
            }

            return (IRepositoryAsync<TEntity>)_repositoriesAsync[type];
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        /// <returns>
        /// Integer result from Save().
        /// </returns>
        public int Save()
        {
            try
            {
                return Context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return -1;
            }
        }

        /// <summary>
        /// Saves the asynchronous.
        /// </summary>
        /// <returns>
        /// Integer result from SaveAsync().
        /// </returns>
        public async Task<int> SaveAsync()
        {
            try
            {
                return await Context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return -1;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="isDisposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        public void Dispose(bool isDisposing)
        {
            if (!_disposed)
            {
                if (isDisposing)
                {
                    Context.Dispose();
                }
            }

            _disposed = true;
        }
    }
}
