// <copyright file="DBContextExtension.cs" company="Sascha Manns">
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
