// <copyright file="20220718102156_AddNewMigration_202207181221501.cs" company="marcos software">
// This file may not be redistributed in whole or significant part
// and is subject to the marcos software license.
//
// @author: Sascha Manns, s.manns@marcossoftware.com
// @copyright: 2022, marcos-software, http://www.marcos-software.de
// </copyright>
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppLicenseserver.Entity.Migrations
{
	public partial class AddNewMigration_202207181221501 : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<bool>(
				name: "IsActive",
				table: "Licenses",
				type: "bit",
				nullable: false,
				defaultValue: false);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "IsActive",
				table: "Licenses");
		}
	}
}
