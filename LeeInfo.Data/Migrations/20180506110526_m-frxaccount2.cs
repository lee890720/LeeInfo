using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace LeeInfo.Data.Migrations
{
    public partial class mfrxaccount2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sequence",
                table: "Frx_Account");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Frx_Account");

            migrationBuilder.CreateTable(
                name: "Frx_UserAccount",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountNumber = table.Column<int>(nullable: false),
                    Password = table.Column<string>(nullable: true),
                    Sequence = table.Column<int>(nullable: false),
                    UserName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Frx_UserAccount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Frx_UserAccount_Frx_Account_AccountNumber",
                        column: x => x.AccountNumber,
                        principalTable: "Frx_Account",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Frx_UserAccount_AccountNumber",
                table: "Frx_UserAccount",
                column: "AccountNumber");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Frx_UserAccount");

            migrationBuilder.AddColumn<int>(
                name: "Sequence",
                table: "Frx_Account",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Frx_Account",
                nullable: true);
        }
    }
}
