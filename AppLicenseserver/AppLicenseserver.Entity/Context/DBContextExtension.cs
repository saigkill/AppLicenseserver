// <copyright file="DBContextExtension.cs" company="marcos software">
//   this file may not be redistributed in whole or significant part
//   and is subject to the marcos software license.
//
//   @author: Sascha Manns, <s.manns@marcossoftware.com>
//   @copyright: 2022, marcos-software.de, http://www.marcos-software.de
// </copyright>

#pragma warning disable SA1027 // TabsMustNotBeUsed
#pragma warning disable SA1200 // UsingDirectivesMustBePlacedWithinNamespace
#pragma warning disable SA1309 // FieldNamesMustNotBeginWithUnderscore
#pragma warning disable SA1101 // PrefixLocalCallsWithThis

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Newtonsoft.Json;

namespace AppLicenseserver.Entity.Context
{
	/// <summary>
	/// Extends the common database context.
	/// </summary>
	public static class DbContextExtension
	{
		/// <summary>
		/// Alls the migrations applied.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns>A total of applied migrations.</returns>
		public static bool AllMigrationsApplied(this DbContext context)
		{
			var applied = context.GetService<IHistoryRepository>()
				.GetAppliedMigrations()
				.Select(m => m.MigrationId);

			var total = context.GetService<IMigrationsAssembly>()
				.Migrations
				.Select(m => m.Key);

			return !total.Except(applied).Any();
		}

		/// <summary>
		/// Ensures the seeded.
		/// </summary>
		/// <param name="context">The context.</param>
		public static void EnsureSeeded(this DefaultDbContext context)
		{
			if (!context.Accounts.Any())
			{
				var accounts = JsonConvert.DeserializeObject<List<Account>>(File.ReadAllText("seed" + Path.DirectorySeparatorChar + "accounts.json"));
				context.AddRange(accounts);
				context.SaveChanges();
			}

			if (!context.Users.Any())
			{
				var users = JsonConvert.DeserializeObject<List<User>>(File.ReadAllText(@"seed" + Path.DirectorySeparatorChar + "users.json"));
				context.AddRange(users);
				context.SaveChanges();
			}

			if (!context.Products.Any())
			{
				var products = JsonConvert.DeserializeObject<List<Product>>(File.ReadAllText(@"seed" + Path.DirectorySeparatorChar + "products.json"));
				context.AddRange(products);
				context.SaveChanges();
			}

			if (!context.Licenses.Any())
			{
				var licenses = JsonConvert.DeserializeObject<List<License>>(File.ReadAllText(@"seed" + Path.DirectorySeparatorChar + "licenses.json"));
				context.AddRange(licenses);
				context.SaveChanges();
			}
		}
	}
}
