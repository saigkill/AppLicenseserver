// <copyright file="DefaultDbContext.cs" company="marcos software">
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
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AppLicenseserver.Entity.Context
{
	/// <summary>
	/// Our current used database context.
	/// </summary>
	/// <seealso cref="Microsoft.EntityFrameworkCore.DbContext" />
	public partial class DefaultDbContext : DbContext
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultDbContext"/> class.
		/// </summary>
		/// <param name="options">The options.</param>
		public DefaultDbContext(DbContextOptions<DefaultDbContext> options)
			: base(options)
		{
		}

		/// <summary>
		/// Gets or sets the accounts.
		/// </summary>
		/// <value>
		/// The accounts.
		/// </value>
		public DbSet<Account> Accounts { get; set; }

		/// <summary>
		/// Gets or sets the users.
		/// </summary>
		/// <value>
		/// The users.
		/// </value>
		public DbSet<User> Users { get; set; }

		// lazy-loading
		// https://entityframeworkcore.com/querying-data-loading-eager-lazy
		// https://docs.microsoft.com/en-us/ef/core/querying/related-data
		// EF Core will enable lazy-loading for any navigation property that is virtual and in an entity class that can be inherited

		/// <summary>
		/// <para>
		/// Override this method to configure the database (and other options) to be used for this context.
		/// This method is called for each instance of the context that is created.
		/// The base implementation does nothing.
		/// </para>
		/// <para>
		/// In situations where an instance of <see cref="T:Microsoft.EntityFrameworkCore.DbContextOptions" /> may or may not have been passed
		/// to the constructor, you can use <see cref="P:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.IsConfigured" /> to determine if
		/// the options have already been set, and skip some or all of the logic in
		/// <see cref="M:Microsoft.EntityFrameworkCore.DbContext.OnConfiguring(Microsoft.EntityFrameworkCore.DbContextOptionsBuilder)" />.
		/// </para>
		/// </summary>
		/// <param name="optionsBuilder">A builder used to create or modify options for this context. Databases (and other extensions)
		/// typically define extension methods on this object that allow you to configure the context.</param>
		/// <remarks>
		/// See <see href="https://aka.ms/efcore-docs-dbcontext">DbContext lifetime, configuration, and initialization</see>
		/// for more information.
		/// </remarks>
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		=> optionsBuilder
		.UseLazyLoadingProxies();

		/// <summary>
		/// Override this method to further configure the model that was discovered by convention from the entity types
		/// exposed in <see cref="T:Microsoft.EntityFrameworkCore.DbSet`1" /> properties on your derived context. The resulting model may be cached
		/// and re-used for subsequent instances of your derived context.
		/// </summary>
		/// <param name="modelBuilder">The builder being used to construct the model for this context. Databases (and other extensions) typically
		/// define extension methods on this object that allow you to configure aspects of the model that are specific
		/// to a given database.</param>
		/// <remarks>
		/// <para>
		/// If a model is explicitly set on the options for this context (via <see cref="M:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.UseModel(Microsoft.EntityFrameworkCore.Metadata.IModel)" />)
		/// then this method will not be run.
		/// </para>
		/// <para>
		/// See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see> for more information.
		/// </para>
		/// </remarks>
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// Fluent API
			modelBuilder.Entity<User>()
		   .HasOne(u => u.Account)
		   .WithMany(e => e.Users);

			// concurrency
			modelBuilder.Entity<Account>()
			.Property(a => a.RowVersion).IsRowVersion();
			modelBuilder.Entity<User>()
			.Property(a => a.RowVersion).IsRowVersion();

			SetAdditionalConcurency(modelBuilder);

			// modelBuilder.Entity<User>()
			// .Property(p => p.DecryptedPassword)
			// .HasComputedColumnSql("Uncrypt(p.PasswordText)");
		}

		// call scaffolded class method to add concurrency

		/// <summary>
		/// Sets the additional concurency.
		/// </summary>
		/// <param name="modelBuilder">The model builder.</param>
		partial void SetAdditionalConcurency(ModelBuilder modelBuilder);

		/// <summary>
		/// Overridden method SaveChanges(), because we want to launch Audit() before.
		/// </summary>
		/// <returns>Integer of returns from SaveChanges().</returns>
		public override int SaveChanges()
		{
			Audit();
			return base.SaveChanges();
		}

		/// <summary>
		/// Overridden method SaveChangesAsync(), because we want to launch Audit() before.
		/// </summary>
		/// <returns>Integer of returns from SaveChanges().</returns>
		public async Task<int> SaveChangesAsync()
		{
			Audit();
			return await base.SaveChangesAsync();
		}

		/// <summary>
		/// Audits (fills entries with some metadata).
		/// </summary>
		private void Audit()
		{
			var entries = ChangeTracker.Entries().Where(x => x.Entity is BaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));
			foreach (var entry in entries)
			{
				if (entry.State == EntityState.Added)
				{
					((BaseEntity)entry.Entity).Created = DateTime.UtcNow;
				}

				((BaseEntity)entry.Entity).Modified = DateTime.UtcNow;
			}
		}
	}
}
