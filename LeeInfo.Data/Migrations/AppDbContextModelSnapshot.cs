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
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.2-rtm-10011")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("LeeInfo.Data.AppSidemenu", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Action")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.Property<string>("Area")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.Property<string>("Controller")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.Property<string>("Description")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.Property<int>("Follow");

                    b.Property<int>("Grade");

                    b.Property<string>("Ico")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.Property<int>("Sequence");

                    b.Property<string>("State")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.Property<string>("Url")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.Property<bool>("Valid");

                    b.HasKey("Id");

                    b.ToTable("App_Sidemenu");
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
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AccountNumber");

                    b.Property<string>("AccountPassword");

                    b.Property<string>("Platform");

                    b.Property<string>("UserName");

                    b.HasKey("Id");

                    b.ToTable("Frx_Account");
                });

            modelBuilder.Entity("LeeInfo.Data.Forex.FrxCbotset", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("Alike")
                        .HasColumnName("alike")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.Property<int>("Average")
                        .HasColumnName("average");

                    b.Property<double>("Brk")
                        .HasColumnName("brk");

                    b.Property<double?>("Ca")
                        .HasColumnName("ca");

                    b.Property<double?>("Cr")
                        .HasColumnName("cr");

                    b.Property<double>("Distance")
                        .HasColumnName("distance");

                    b.Property<int>("Initvolume")
                        .HasColumnName("initvolume");

                    b.Property<bool>("Isbreak")
                        .HasColumnName("isbreak");

                    b.Property<bool>("Isbrkfirst")
                        .HasColumnName("isbrkfirst");

                    b.Property<bool>("Istrade")
                        .HasColumnName("istrade");

                    b.Property<double>("Magnify")
                        .HasColumnName("magnify");

                    b.Property<int>("Result")
                        .HasColumnName("result");

                    b.Property<double?>("Sa")
                        .HasColumnName("sa");

                    b.Property<string>("Signal")
                        .HasColumnName("signal")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.Property<double?>("Sr")
                        .HasColumnName("sr");

                    b.Property<double>("Sub")
                        .HasColumnName("sub");

                    b.Property<string>("Symbol")
                        .IsRequired()
                        .HasColumnName("symbol")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.Property<int>("Tmr")
                        .HasColumnName("tmr");

                    b.HasKey("Id");

                    b.ToTable("Frx_Cbotset");
                });

            modelBuilder.Entity("LeeInfo.Data.Forex.FrxEcs", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("EcsName");

                    b.Property<DateTime>("EcsTime");

                    b.HasKey("Id");

                    b.ToTable("Frx_Ecs");
                });

            modelBuilder.Entity("LeeInfo.Data.Forex.FrxHistory", b =>
                {
                    b.Property<int>("ClosingDealId");

                    b.Property<double>("Balance");

                    b.Property<double>("ClosingPrice");

                    b.Property<DateTime>("ClosingTime");

                    b.Property<string>("Comment");

                    b.Property<double>("Commissions");

                    b.Property<double>("EntryPrice");

                    b.Property<DateTime>("EntryTime");

                    b.Property<int?>("FrxAccountId");

                    b.Property<double>("GrossProfit");

                    b.Property<string>("Label");

                    b.Property<double>("NetProfit");

                    b.Property<double>("Pips");

                    b.Property<int>("PositionId");

                    b.Property<double>("Quantity");

                    b.Property<double>("Swap");

                    b.Property<string>("SymbolCode");

                    b.Property<int>("TradeType");

                    b.Property<long>("Volume");

                    b.HasKey("ClosingDealId");

                    b.HasIndex("FrxAccountId");

                    b.ToTable("Frx_History");
                });

            modelBuilder.Entity("LeeInfo.Data.Forex.FrxPosition", b =>
                {
                    b.Property<int>("Id");

                    b.Property<string>("Comment");

                    b.Property<double>("Commissions");

                    b.Property<DateTime>("EntryTime");

                    b.Property<int?>("FrxAccountId");

                    b.Property<double>("GrossProfit");

                    b.Property<string>("Label");

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

                    b.HasIndex("FrxAccountId");

                    b.ToTable("Frx_Position");
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

            modelBuilder.Entity("LeeInfo.Data.Forex.FrxHistory", b =>
                {
                    b.HasOne("LeeInfo.Data.Forex.FrxAccount", "FrxAccount")
                        .WithMany("FrxHistory")
                        .HasForeignKey("FrxAccountId");
                });

            modelBuilder.Entity("LeeInfo.Data.Forex.FrxPosition", b =>
                {
                    b.HasOne("LeeInfo.Data.Forex.FrxAccount", "FrxAccount")
                        .WithMany("FrxPosition")
                        .HasForeignKey("FrxAccountId");
                });
#pragma warning restore 612, 618
        }
    }
}
