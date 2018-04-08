using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace LeeInfo.Data.Migrations
{
    public partial class DelFrxAccountData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Frx_AccountData");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Frx_AccountData",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountNumber = table.Column<int>(nullable: false),
                    Balance = table.Column<string>(nullable: true),
                    DataTime = table.Column<DateTime>(nullable: false),
                    Equity = table.Column<string>(nullable: true),
                    TimeFrame = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Frx_AccountData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Frx_AccountData_Frx_Account_AccountNumber",
                        column: x => x.AccountNumber,
                        principalTable: "Frx_Account",
                        principalColumn: "AccountNumber",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Frx_AccountData_AccountNumber",
                table: "Frx_AccountData",
                column: "AccountNumber");
        }
    }
}
