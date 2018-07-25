﻿// <auto-generated />
using LeeInfo.Data;
using LeeInfo.Data.CreditCard;
using LeeInfo.Data.Forex;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace LeeInfo.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20180531105411_add-srsa")]
    partial class addsrsa
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.2-rtm-10011")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("LeeInfo.Data.AppContact", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content");

                    b.Property<bool>("Dispose");

                    b.Property<string>("Mail");

                    b.Property<string>("Phone");

                    b.Property<string>("Title");

                    b.Property<string>("UserName");

                    b.HasKey("Id");

                    b.ToTable("App_Contact");
                });

            modelBuilder.Entity("LeeInfo.Data.AppMenu", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Action");

                    b.Property<string>("Area");

                    b.Property<string>("Controller");

                    b.Property<string>("Description");

                    b.Property<int>("Follow");

                    b.Property<int>("Grade");

                    b.Property<string>("Ico");

                    b.Property<string>("Name");

                    b.Property<int>("Sequence");

                    b.Property<string>("State");

                    b.Property<string>("Url");

                    b.Property<bool>("Valid");

                    b.HasKey("Id");

                    b.ToTable("App_Menu");
                });

            modelBuilder.Entity("LeeInfo.Data.CreditCard.CcdBill", b =>
                {
                    b.Property<int>("BillId")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("BillAmount");

                    b.Property<DateTime>("BillDate")
                        .HasColumnType("datetime");

                    b.Property<int>("CreditCardId");

                    b.HasKey("BillId");

                    b.HasIndex("CreditCardId")
                        .HasName("IX_CreditCardID");

                    b.ToTable("Ccd_Bill");
                });

            modelBuilder.Entity("LeeInfo.Data.CreditCard.CcdData", b =>
                {
                    b.Property<int>("CreditCardId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("AccountBill")
                        .HasColumnType("datetime");

                    b.Property<double>("BillAmount");

                    b.Property<string>("CreditCardNumber")
                        .IsRequired();

                    b.Property<string>("Cvv")
                        .IsRequired()
                        .HasColumnName("CVV");

                    b.Property<double>("HasPayment")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("((0))");

                    b.Property<string>("InquriyPw")
                        .IsRequired()
                        .HasColumnName("InquriyPW")
                        .HasMaxLength(6);

                    b.Property<int>("IssuingBank");

                    b.Property<double>("Limit");

                    b.Property<string>("OnlineBankingPw")
                        .HasColumnName("OnlineBankingPW");

                    b.Property<int>("PersonId");

                    b.Property<double>("PrePayment")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("((0))");

                    b.Property<DateTime>("RepaymentDate")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("TempDate")
                        .HasColumnType("datetime");

                    b.Property<double?>("Temporary");

                    b.Property<string>("TransactionPw")
                        .IsRequired()
                        .HasColumnName("TransactionPW")
                        .HasMaxLength(6);

                    b.Property<string>("ValidThru")
                        .IsRequired();

                    b.HasKey("CreditCardId");

                    b.HasIndex("PersonId")
                        .HasName("IX_PersonID");

                    b.ToTable("Ccd_Data");
                });

            modelBuilder.Entity("LeeInfo.Data.CreditCard.CcdDebt", b =>
                {
                    b.Property<int>("DebtId")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("BillAmount");

                    b.Property<double>("CurrentAmount")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("((0))");

                    b.Property<string>("DebtNote");

                    b.Property<string>("DebtTitle")
                        .IsRequired();

                    b.Property<double>("InterestRate")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("((0))");

                    b.Property<int>("PersonId");

                    b.Property<DateTime>("RepaymentDate")
                        .HasColumnType("datetime");

                    b.HasKey("DebtId");

                    b.HasIndex("PersonId")
                        .HasName("IX_PersonID");

                    b.ToTable("Ccd_Debt");
                });

            modelBuilder.Entity("LeeInfo.Data.CreditCard.CcdPerson", b =>
                {
                    b.Property<int>("PersonId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("DateOfBirth")
                        .HasColumnType("datetime");

                    b.Property<string>("Email");

                    b.Property<string>("IdcardNumber")
                        .HasColumnName("IDCardNumber");

                    b.Property<string>("Mobile")
                        .IsRequired();

                    b.Property<string>("PersonName")
                        .IsRequired();

                    b.Property<string>("PersonNote");

                    b.Property<int>("Sex");

                    b.HasKey("PersonId");

                    b.ToTable("Ccd_Person");
                });

            modelBuilder.Entity("LeeInfo.Data.CreditCard.CcdPos", b =>
                {
                    b.Property<int>("PosId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("PersonId");

                    b.Property<string>("PosName")
                        .IsRequired();

                    b.Property<string>("PosNote");

                    b.HasKey("PosId");

                    b.HasIndex("PersonId")
                        .HasName("IX_PersonID");

                    b.ToTable("Ccd_Pos");
                });

            modelBuilder.Entity("LeeInfo.Data.CreditCard.CcdRecord", b =>
                {
                    b.Property<int>("RecordId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CreditCardId");

                    b.Property<double?>("Deposit");

                    b.Property<double?>("Expend");

                    b.Property<int?>("PosId");

                    b.Property<DateTime>("RecordDate")
                        .HasColumnType("datetime");

                    b.HasKey("RecordId");

                    b.HasIndex("CreditCardId")
                        .HasName("IX_CreditCardID");

                    b.HasIndex("PosId")
                        .HasName("IX_POSID");

                    b.ToTable("Ccd_Record");
                });

            modelBuilder.Entity("LeeInfo.Data.Forex.FrxAccount", b =>
                {
                    b.Property<int>("AccountId");

                    b.Property<string>("AccessToken");

                    b.Property<int>("AccountNumber");

                    b.Property<string>("ApiHost");

                    b.Property<int>("ApiPort");

                    b.Property<string>("ApiUrl");

                    b.Property<double>("Balance");

                    b.Property<string>("BrokerName");

                    b.Property<string>("ClientId");

                    b.Property<string>("ClientSecret");

                    b.Property<string>("ConnectUrl");

                    b.Property<string>("Currency");

                    b.Property<double>("Equity");

                    b.Property<double>("FreeMargin");

                    b.Property<bool>("IsLive");

                    b.Property<double>("MarginLevel");

                    b.Property<double>("MarginUsed");

                    b.Property<string>("Password");

                    b.Property<double>("PreciseLeverage");

                    b.Property<string>("RefreshToken");

                    b.Property<long>("TraderRegistrationTimestamp");

                    b.Property<double>("UnrealizedNetProfit");

                    b.HasKey("AccountId");

                    b.ToTable("Frx_Account");
                });

            modelBuilder.Entity("LeeInfo.Data.Forex.FrxCashflow", b =>
                {
                    b.Property<int>("Id");

                    b.Property<int>("AccountId");

                    b.Property<double>("Balance");

                    b.Property<long>("BalanceVersion");

                    b.Property<long>("ChangeTimestamp");

                    b.Property<double>("Delta");

                    b.Property<double>("Equity");

                    b.Property<string>("Type");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.ToTable("Frx_Cashflow");
                });

            modelBuilder.Entity("LeeInfo.Data.Forex.FrxCbotset", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Average");

                    b.Property<double>("Brk");

                    b.Property<double?>("Ca");

                    b.Property<double?>("Cr");

                    b.Property<double>("Distance");

                    b.Property<int>("InitVolume");

                    b.Property<bool>("IsBreak");

                    b.Property<bool>("IsBrkFirst");

                    b.Property<bool>("IsTrade");

                    b.Property<double>("Magnify");

                    b.Property<int>("Result");

                    b.Property<double?>("Sa");

                    b.Property<string>("Signal");

                    b.Property<string>("Signal2");

                    b.Property<double?>("Sr");

                    b.Property<double?>("SrSa");

                    b.Property<double>("Sub");

                    b.Property<string>("Symbol");

                    b.Property<int>("Tmr");

                    b.HasKey("Id");

                    b.ToTable("Frx_Cbotset");
                });

            modelBuilder.Entity("LeeInfo.Data.Forex.FrxHistory", b =>
                {
                    b.Property<int>("ClosingDealId");

                    b.Property<int>("AccountId");

                    b.Property<double>("Balance");

                    b.Property<int>("BalanceVersion");

                    b.Property<double>("BaseToUSDConversionRate");

                    b.Property<double?>("ClosedToDepoitConversionRate");

                    b.Property<double>("ClosingPrice");

                    b.Property<long>("ClosingTimestamp");

                    b.Property<string>("Comment");

                    b.Property<double>("Commissions");

                    b.Property<double>("EntryPrice");

                    b.Property<long>("EntryTimestamp");

                    b.Property<double>("Equity");

                    b.Property<double>("EquityBaseRoi");

                    b.Property<double>("GrossProfit");

                    b.Property<string>("Label");

                    b.Property<double>("Lot");

                    b.Property<double>("MarginRate");

                    b.Property<double>("NetProfit");

                    b.Property<int?>("PipPosition");

                    b.Property<double>("Pips");

                    b.Property<int>("PositionId");

                    b.Property<double?>("QuoteToDepositConversionRate");

                    b.Property<double>("Roi");

                    b.Property<double>("Swap");

                    b.Property<string>("SymbolCode");

                    b.Property<int>("TradeType");

                    b.Property<long>("Volume");

                    b.HasKey("ClosingDealId");

                    b.HasIndex("AccountId");

                    b.ToTable("Frx_History");
                });

            modelBuilder.Entity("LeeInfo.Data.Forex.FrxPosition", b =>
                {
                    b.Property<int>("Id");

                    b.Property<int>("AccountId");

                    b.Property<string>("Channel");

                    b.Property<string>("Comment");

                    b.Property<double>("Commissions");

                    b.Property<double>("CurrentPrice");

                    b.Property<int?>("Digits");

                    b.Property<double>("EntryPrice");

                    b.Property<long>("EntryTimestamp");

                    b.Property<double>("GrossProfit");

                    b.Property<string>("Label");

                    b.Property<double>("Margin");

                    b.Property<double>("MarginRate");

                    b.Property<double>("NetProfit");

                    b.Property<double>("Pips");

                    b.Property<double>("Quantity");

                    b.Property<double?>("StopLoss");

                    b.Property<double>("Swap");

                    b.Property<string>("SymbolCode");

                    b.Property<double?>("TakeProfit");

                    b.Property<int>("TradeType");

                    b.Property<long>("Volume");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.ToTable("Frx_Position");
                });

            modelBuilder.Entity("LeeInfo.Data.Forex.FrxServer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccountNumber");

                    b.Property<string>("ServerName");

                    b.Property<DateTime>("ServerTime");

                    b.HasKey("Id");

                    b.ToTable("Frx_Server");
                });

            modelBuilder.Entity("LeeInfo.Data.Forex.FrxSymbol", b =>
                {
                    b.Property<int>("SymbolId");

                    b.Property<int>("AssetClass");

                    b.Property<string>("BaseAsset");

                    b.Property<string>("Description");

                    b.Property<int>("Digits");

                    b.Property<double?>("LastAsk");

                    b.Property<double?>("LastBid");

                    b.Property<int>("MaxLeverage");

                    b.Property<long>("MaxOrderVolume");

                    b.Property<string>("MeasurementUnits");

                    b.Property<double>("MinOrderLot");

                    b.Property<long>("MinOrderStep");

                    b.Property<long>("MinOrderVolume");

                    b.Property<int>("PipPosition");

                    b.Property<string>("QuoteAsset");

                    b.Property<double>("SwapLong");

                    b.Property<double>("SwapShort");

                    b.Property<string>("SymbolName");

                    b.Property<string>("ThreeDaysSwaps");

                    b.Property<double>("TickSize");

                    b.Property<bool>("TradeEnabled");

                    b.Property<string>("TradingMode");

                    b.HasKey("SymbolId");

                    b.ToTable("Frx_Symbol");
                });

            modelBuilder.Entity("LeeInfo.Data.CreditCard.CcdBill", b =>
                {
                    b.HasOne("LeeInfo.Data.CreditCard.CcdData", "CcdData")
                        .WithMany("CcdBill")
                        .HasForeignKey("CreditCardId")
                        .HasConstraintName("FK_dbo.Ccd_Bill_dbo.Ccd_Data_CreditCardId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("LeeInfo.Data.CreditCard.CcdData", b =>
                {
                    b.HasOne("LeeInfo.Data.CreditCard.CcdPerson", "CcdPerson")
                        .WithMany("CcdData")
                        .HasForeignKey("PersonId")
                        .HasConstraintName("FK_dbo.Ccd_Data_dbo.Ccd_Person_PersonId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("LeeInfo.Data.CreditCard.CcdDebt", b =>
                {
                    b.HasOne("LeeInfo.Data.CreditCard.CcdPerson", "CcdPerson")
                        .WithMany("CcdDebt")
                        .HasForeignKey("PersonId")
                        .HasConstraintName("FK_dbo.Ccd_Debt_dbo.Ccd_Person_PersonId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("LeeInfo.Data.CreditCard.CcdPos", b =>
                {
                    b.HasOne("LeeInfo.Data.CreditCard.CcdPerson", "CcdPerson")
                        .WithMany("CcdPos")
                        .HasForeignKey("PersonId")
                        .HasConstraintName("FK_dbo.Ccd_Pos_dbo.Ccd_Person_PersonId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("LeeInfo.Data.CreditCard.CcdRecord", b =>
                {
                    b.HasOne("LeeInfo.Data.CreditCard.CcdData", "CcdData")
                        .WithMany("CcdRecord")
                        .HasForeignKey("CreditCardId")
                        .HasConstraintName("FK_dbo.Ccd_Record_dbo.Ccd_Data_CreditCardId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("LeeInfo.Data.CreditCard.CcdPos", "CcdPos")
                        .WithMany("CcdRecord")
                        .HasForeignKey("PosId")
                        .HasConstraintName("FK_dbo.Ccd_Record_dbo.Ccd_Pos_PosId");
                });

            modelBuilder.Entity("LeeInfo.Data.Forex.FrxCashflow", b =>
                {
                    b.HasOne("LeeInfo.Data.Forex.FrxAccount", "FrxAccount")
                        .WithMany("FrxCashflow")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("LeeInfo.Data.Forex.FrxHistory", b =>
                {
                    b.HasOne("LeeInfo.Data.Forex.FrxAccount", "FrxAccount")
                        .WithMany("FrxHistory")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("LeeInfo.Data.Forex.FrxPosition", b =>
                {
                    b.HasOne("LeeInfo.Data.Forex.FrxAccount", "FrxAccount")
                        .WithMany("FrxPosition")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
