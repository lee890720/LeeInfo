using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace LeeInfo.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "App_Menu",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Action = table.Column<string>(nullable: true),
                    Area = table.Column<string>(nullable: true),
                    Controller = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Follow = table.Column<int>(nullable: false),
                    Grade = table.Column<int>(nullable: false),
                    Ico = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Sequence = table.Column<int>(nullable: false),
                    State = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true),
                    Valid = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_App_Menu", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ccd_Person",
                columns: table => new
                {
                    PersonId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateOfBirth = table.Column<DateTime>(type: "datetime", nullable: true),
                    Email = table.Column<string>(nullable: true),
                    IDCardNumber = table.Column<string>(nullable: true),
                    Mobile = table.Column<string>(nullable: false),
                    PersonName = table.Column<string>(nullable: false),
                    PersonNote = table.Column<string>(nullable: true),
                    Sex = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ccd_Person", x => x.PersonId);
                });

            migrationBuilder.CreateTable(
                name: "Frx_Account",
                columns: table => new
                {
                    AccountNumber = table.Column<int>(nullable: false),
                    Balance = table.Column<double>(nullable: false),
                    BrokerName = table.Column<string>(nullable: true),
                    Currency = table.Column<string>(nullable: true),
                    Equity = table.Column<double>(nullable: false),
                    IsLive = table.Column<bool>(nullable: false),
                    Password = table.Column<string>(nullable: true),
                    PreciseLeverage = table.Column<double>(nullable: false),
                    UnrealizedNetProfit = table.Column<double>(nullable: false),
                    UserName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Frx_Account", x => x.AccountNumber);
                });

            migrationBuilder.CreateTable(
                name: "Frx_Cbotset",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Alike = table.Column<string>(nullable: true),
                    Average = table.Column<int>(nullable: false),
                    Brk = table.Column<double>(nullable: false),
                    Ca = table.Column<double>(nullable: true),
                    Cr = table.Column<double>(nullable: true),
                    Distance = table.Column<double>(nullable: false),
                    InitVolume = table.Column<int>(nullable: false),
                    IsBreak = table.Column<bool>(nullable: false),
                    IsBrkFirst = table.Column<bool>(nullable: false),
                    IsTrade = table.Column<bool>(nullable: false),
                    Magnify = table.Column<double>(nullable: false),
                    Result = table.Column<int>(nullable: false),
                    Sa = table.Column<double>(nullable: true),
                    Signal = table.Column<string>(nullable: true),
                    Sr = table.Column<double>(nullable: true),
                    Sub = table.Column<double>(nullable: false),
                    Symbol = table.Column<string>(nullable: true),
                    Tmr = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Frx_Cbotset", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Frx_Server",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountNumber = table.Column<int>(nullable: false),
                    ServerName = table.Column<string>(nullable: true),
                    ServerTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Frx_Server", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ccd_Data",
                columns: table => new
                {
                    CreditCardId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountBill = table.Column<DateTime>(type: "datetime", nullable: false),
                    BillAmount = table.Column<double>(nullable: false),
                    CreditCardNumber = table.Column<string>(nullable: false),
                    CVV = table.Column<string>(nullable: false),
                    HasPayment = table.Column<double>(nullable: false, defaultValueSql: "((0))"),
                    InquriyPW = table.Column<string>(maxLength: 6, nullable: false),
                    IssuingBank = table.Column<int>(nullable: false),
                    Limit = table.Column<double>(nullable: false),
                    OnlineBankingPW = table.Column<string>(nullable: true),
                    PersonId = table.Column<int>(nullable: false),
                    PrePayment = table.Column<double>(nullable: false, defaultValueSql: "((0))"),
                    RepaymentDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    TempDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Temporary = table.Column<double>(nullable: true),
                    TransactionPW = table.Column<string>(maxLength: 6, nullable: false),
                    ValidThru = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ccd_Data", x => x.CreditCardId);
                    table.ForeignKey(
                        name: "FK_dbo.Ccd_Data_dbo.Ccd_Person_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Ccd_Person",
                        principalColumn: "PersonId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ccd_Debt",
                columns: table => new
                {
                    DebtId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BillAmount = table.Column<double>(nullable: false),
                    CurrentAmount = table.Column<double>(nullable: false, defaultValueSql: "((0))"),
                    DebtNote = table.Column<string>(nullable: true),
                    DebtTitle = table.Column<string>(nullable: false),
                    InterestRate = table.Column<double>(nullable: false, defaultValueSql: "((0))"),
                    PersonId = table.Column<int>(nullable: false),
                    RepaymentDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ccd_Debt", x => x.DebtId);
                    table.ForeignKey(
                        name: "FK_dbo.Ccd_Debt_dbo.Ccd_Person_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Ccd_Person",
                        principalColumn: "PersonId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ccd_Pos",
                columns: table => new
                {
                    PosId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PersonId = table.Column<int>(nullable: false),
                    PosName = table.Column<string>(nullable: false),
                    PosNote = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ccd_Pos", x => x.PosId);
                    table.ForeignKey(
                        name: "FK_dbo.Ccd_Pos_dbo.Ccd_Person_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Ccd_Person",
                        principalColumn: "PersonId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Frx_AccountData",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountNumber = table.Column<int>(nullable: false),
                    Balance = table.Column<string>(nullable: true),
                    Enquey = table.Column<string>(nullable: true)
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

            migrationBuilder.CreateTable(
                name: "Frx_History",
                columns: table => new
                {
                    ClosingDealId = table.Column<int>(nullable: false),
                    AccountNumber = table.Column<int>(nullable: false),
                    Balance = table.Column<double>(nullable: false),
                    ClosingPrice = table.Column<double>(nullable: false),
                    ClosingTime = table.Column<DateTime>(nullable: false),
                    Comment = table.Column<string>(nullable: true),
                    Commissions = table.Column<double>(nullable: false),
                    EntryPrice = table.Column<double>(nullable: false),
                    EntryTime = table.Column<DateTime>(nullable: false),
                    GrossProfit = table.Column<double>(nullable: false),
                    Label = table.Column<string>(nullable: true),
                    NetProfit = table.Column<double>(nullable: false),
                    Pips = table.Column<double>(nullable: false),
                    PositionId = table.Column<int>(nullable: false),
                    Quantity = table.Column<double>(nullable: false),
                    Swap = table.Column<double>(nullable: false),
                    SymbolCode = table.Column<string>(nullable: true),
                    TradeType = table.Column<int>(nullable: false),
                    Volume = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Frx_History", x => x.ClosingDealId);
                    table.ForeignKey(
                        name: "FK_Frx_History_Frx_Account_AccountNumber",
                        column: x => x.AccountNumber,
                        principalTable: "Frx_Account",
                        principalColumn: "AccountNumber",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Frx_Position",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    AccountNumber = table.Column<int>(nullable: false),
                    Comment = table.Column<string>(nullable: true),
                    Commissions = table.Column<double>(nullable: false),
                    EntryPrice = table.Column<double>(nullable: false),
                    EntryTime = table.Column<DateTime>(nullable: false),
                    GrossProfit = table.Column<double>(nullable: false),
                    Label = table.Column<string>(nullable: true),
                    NetProfit = table.Column<double>(nullable: false),
                    Pips = table.Column<double>(nullable: false),
                    Quantity = table.Column<double>(nullable: false),
                    StopLoss = table.Column<double>(nullable: true),
                    Swap = table.Column<double>(nullable: false),
                    SymbolCode = table.Column<string>(nullable: true),
                    TakeProfit = table.Column<double>(nullable: true),
                    TradeType = table.Column<int>(nullable: false),
                    Volume = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Frx_Position", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Frx_Position_Frx_Account_AccountNumber",
                        column: x => x.AccountNumber,
                        principalTable: "Frx_Account",
                        principalColumn: "AccountNumber",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ccd_Bill",
                columns: table => new
                {
                    BillId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BillAmount = table.Column<double>(nullable: false),
                    BillDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreditCardId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ccd_Bill", x => x.BillId);
                    table.ForeignKey(
                        name: "FK_dbo.Ccd_Bill_dbo.Ccd_Data_CreditCardId",
                        column: x => x.CreditCardId,
                        principalTable: "Ccd_Data",
                        principalColumn: "CreditCardId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ccd_Record",
                columns: table => new
                {
                    RecordId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreditCardId = table.Column<int>(nullable: false),
                    Deposit = table.Column<double>(nullable: true),
                    Expend = table.Column<double>(nullable: true),
                    PosId = table.Column<int>(nullable: true),
                    RecordDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ccd_Record", x => x.RecordId);
                    table.ForeignKey(
                        name: "FK_dbo.Ccd_Record_dbo.Ccd_Data_CreditCardId",
                        column: x => x.CreditCardId,
                        principalTable: "Ccd_Data",
                        principalColumn: "CreditCardId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.Ccd_Record_dbo.Ccd_Pos_PosId",
                        column: x => x.PosId,
                        principalTable: "Ccd_Pos",
                        principalColumn: "PosId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardID",
                table: "Ccd_Bill",
                column: "CreditCardId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonID",
                table: "Ccd_Data",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonID",
                table: "Ccd_Debt",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonID",
                table: "Ccd_Pos",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardID",
                table: "Ccd_Record",
                column: "CreditCardId");

            migrationBuilder.CreateIndex(
                name: "IX_POSID",
                table: "Ccd_Record",
                column: "PosId");

            migrationBuilder.CreateIndex(
                name: "IX_Frx_AccountData_AccountNumber",
                table: "Frx_AccountData",
                column: "AccountNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Frx_History_AccountNumber",
                table: "Frx_History",
                column: "AccountNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Frx_Position_AccountNumber",
                table: "Frx_Position",
                column: "AccountNumber");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "App_Menu");

            migrationBuilder.DropTable(
                name: "Ccd_Bill");

            migrationBuilder.DropTable(
                name: "Ccd_Debt");

            migrationBuilder.DropTable(
                name: "Ccd_Record");

            migrationBuilder.DropTable(
                name: "Frx_AccountData");

            migrationBuilder.DropTable(
                name: "Frx_Cbotset");

            migrationBuilder.DropTable(
                name: "Frx_History");

            migrationBuilder.DropTable(
                name: "Frx_Position");

            migrationBuilder.DropTable(
                name: "Frx_Server");

            migrationBuilder.DropTable(
                name: "Ccd_Data");

            migrationBuilder.DropTable(
                name: "Ccd_Pos");

            migrationBuilder.DropTable(
                name: "Frx_Account");

            migrationBuilder.DropTable(
                name: "Ccd_Person");
        }
    }
}
