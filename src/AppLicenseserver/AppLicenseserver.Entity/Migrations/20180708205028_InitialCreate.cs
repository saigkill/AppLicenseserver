using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace AppLicenseserver.Entity.Migrations
{
	public partial class InitialCreate : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "Accounts",
				columns: table => new
				{
					Id = table.Column<int>(nullable: false)
						.Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
					Created = table.Column<DateTime>(nullable: false),
					Modified = table.Column<DateTime>(nullable: false),
					Name = table.Column<string>(maxLength: 30, nullable: false),
					Email = table.Column<string>(maxLength: 30, nullable: false),
					Description = table.Column<string>(maxLength: 255, nullable: true),
					IsTrial = table.Column<bool>(nullable: false),
					IsActive = table.Column<bool>(nullable: false),
					SetActive = table.Column<DateTime>(nullable: false),
					RowVersion = table.Column<byte[]>(nullable: true, rowVersion: true),
					TestText = table.Column<string>(maxLength: 50, nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Accounts", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "Users",
				columns: table => new
				{
					Id = table.Column<int>(nullable: false)
						.Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
					Created = table.Column<DateTime>(nullable: false),
					Modified = table.Column<DateTime>(nullable: false),
					FirstName = table.Column<string>(maxLength: 20, nullable: false),
					LastName = table.Column<string>(maxLength: 20, nullable: false),
					UserName = table.Column<string>(maxLength: 30, nullable: true),
					Email = table.Column<string>(maxLength: 30, nullable: false),
					Description = table.Column<string>(maxLength: 255, nullable: true),
					IsAdminRole = table.Column<bool>(nullable: false),
					Roles = table.Column<string>(maxLength: 255, nullable: true),
					IsActive = table.Column<bool>(nullable: false),
					Password = table.Column<string>(maxLength: 255, nullable: true),
					AccountId = table.Column<int>(nullable: false),
					RowVersion = table.Column<byte[]>(nullable: true, rowVersion: true),
					TestText = table.Column<string>(maxLength: 50, nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Users", x => x.Id);
					table.ForeignKey(
						name: "FK_Users_Accounts_AccountId",
						column: x => x.AccountId,
						principalTable: "Accounts",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateIndex(
				name: "IX_Users_AccountId",
				table: "Users",
				column: "AccountId");

			//stored procedure example			
			migrationBuilder.Sql(@"
    -- =============================================
    -- Author:      <Autor, Sascha Manns, Marcos Software>
    -- Create Date: <Create Date, 22/04/2022>
    -- Description: <Description, This procedure updates a email adress from a given username>
    -- =============================================
    CREATE PROCEDURE [dbo].[prUpdateEmailByUsername]
    (	  
        @UserName nvarchar(50),
        @Email nvarchar(50)
    )
    AS   
    BEGIN
        UPDATE Users
        SET Email=@Email
        WHERE UserName = @UserName;  
    END
                            ");

			migrationBuilder.Sql(@"
    -- =============================================
    -- Author:      <Autor, Sascha Manns, Marcos Software>
    -- Create Date: <Create Date, 22/04/2022>
    -- Description: <Description, This procedure removes a user by its username>
    -- =============================================
    CREATE PROCEDURE [dbo].[prDeleteUserTrash]
    (	  
        @UserName varchar(50)
    )   
    AS   
    BEGIN
        DELETE from Users
        WHERE UserName = '@UserName';		
    END
                            ");

			migrationBuilder.Sql(@"
    -- =============================================
    -- Author:      <Autor, Sascha Manns, Marcos Software>
    -- Create Date: <Create Date, 22/04/2022>
    -- Description: <Description, This procedure removes test accounts created by Unit Tests>
    -- =============================================
    CREATE PROCEDURE [dbo].[prDeleteAccountTrash]
    (	  
        @Email varchar(50)
    )   
    AS   
    BEGIN
        DELETE from Accounts
        WHERE Email = '@Email';		
    END
                            ");

			migrationBuilder.Sql(@"
    -- =============================================
    -- Author:      <Autor, Sascha Manns, Marcos Software>
    -- Create Date: <Create Date, 22/04/2022>
    -- Description: <Description, This procedure removes a license by its licensenumber>
    -- =============================================
    CREATE PROCEDURE [dbo].[prDeleteLicensenumber]
    (	  
        @Licensenumber varchar(50)
    )   
    AS   
    BEGIN
        DELETE from Licenses
        WHERE Licensenumber = '@Licensenumber';
    END
                            ");

		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "Users");

			migrationBuilder.DropTable(
				name: "Accounts");

			migrationBuilder.Sql(@"
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('dbo.prUpdateEmailByUsername'))
BEGIN
DROP PROCEDURE prUpdateEmailByUsername   
END
GO 
                            ");

			migrationBuilder.Sql(@"
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('dbo.prDeleteUserTrash'))
BEGIN
DROP PROCEDURE prDeleteUserTrash
END
GO 
                            ");

			migrationBuilder.Sql(@"
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('dbo.prDeleteAccountTrash'))
BEGIN
DROP PROCEDURE prDeleteAccountTrash
END
GO 
                            ");

			migrationBuilder.Sql(@"
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('dbo.prDeleteLicensenumber'))
BEGIN
DROP PROCEDURE prDeleteLicensenumber
END
GO 
                            ");
		}
	}
}
