using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using LeeInfo.Data.Forex;
using LeeInfo.Data.CreditCard;

namespace LeeInfo.Data
{
    public partial class AppDbContext : DbContext
    {
        public virtual DbSet<AppSidemenu> AppSidemenu { get; set; }
        public virtual DbSet<FrxCbotset> FrxCbotset { get; set; }
        public virtual DbSet<FrxEcs> FrxEcs { get; set; }
        public virtual DbSet<CcdBill> CcdBill { get; set; }
        public virtual DbSet<CcdData> CcdData { get; set; }
        public virtual DbSet<CcdDebt> CcdDebt { get; set; }
        public virtual DbSet<CcdPerson> CcdPerson { get; set; }
        public virtual DbSet<CcdPos> CcdPos { get; set; }
        public virtual DbSet<CcdRecord> CcdRecord { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppSidemenu>(entity =>
            {
                entity.ToTable("App_Sidemenu");

                entity.Property(e => e.Action)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Area)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Controller)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Description)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Ico)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.State)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Url)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<FrxCbotset>(entity =>
            {
                entity.ToTable("Frx_Cbotset");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Alike)
                    .HasColumnName("alike")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Average).HasColumnName("average");

                entity.Property(e => e.Brk).HasColumnName("brk");

                entity.Property(e => e.Ca).HasColumnName("ca");

                entity.Property(e => e.Cr).HasColumnName("cr");

                entity.Property(e => e.Distance).HasColumnName("distance");

                entity.Property(e => e.Initvolume).HasColumnName("initvolume");

                entity.Property(e => e.Isbreak).HasColumnName("isbreak");

                entity.Property(e => e.Isbrkfirst).HasColumnName("isbrkfirst");

                entity.Property(e => e.Istrade).HasColumnName("istrade");

                entity.Property(e => e.Magnify).HasColumnName("magnify");

                entity.Property(e => e.Result).HasColumnName("result");

                entity.Property(e => e.Sa).HasColumnName("sa");

                entity.Property(e => e.Signal)
                    .HasColumnName("signal")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Sr).HasColumnName("sr");

                entity.Property(e => e.Sub).HasColumnName("sub");

                entity.Property(e => e.Symbol)
                    .IsRequired()
                    .HasColumnName("symbol")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Tmr).HasColumnName("tmr");
            });
            
            modelBuilder.Entity<FrxEcs>(entity=>
            {
                entity.ToTable("Frx_Ecs");
            });

            modelBuilder.Entity<CcdBill>(entity =>
            {
                entity.HasKey(e => e.BillId);

                entity.ToTable("Ccd_Bill");

                entity.HasIndex(e => e.CreditCardId)
                    .HasName("IX_CreditCardID");

                entity.Property(e => e.BillDate).HasColumnType("datetime");

                entity.HasOne(d => d.CcdData)
                    .WithMany(p => p.CcdBill)
                    .HasForeignKey(d => d.CreditCardId)
                    .HasConstraintName("FK_dbo.Ccd_Bill_dbo.Ccd_Data_CreditCardId");
            });

            modelBuilder.Entity<CcdData>(entity =>
            {
                entity.HasKey(e => e.CreditCardId);

                entity.ToTable("Ccd_Data");

                entity.HasIndex(e => e.PersonId)
                    .HasName("IX_PersonID");

                entity.Property(e => e.AccountBill).HasColumnType("datetime");

                entity.Property(e => e.CreditCardNumber).IsRequired();

                entity.Property(e => e.Cvv)
                    .IsRequired()
                    .HasColumnName("CVV");

                entity.Property(e => e.HasPayment).HasDefaultValueSql("((0))");

                entity.Property(e => e.InquriyPw)
                    .IsRequired()
                    .HasColumnName("InquriyPW")
                    .HasMaxLength(6);

                entity.Property(e => e.OnlineBankingPw).HasColumnName("OnlineBankingPW");

                entity.Property(e => e.PrePayment).HasDefaultValueSql("((0))");

                entity.Property(e => e.RepaymentDate).HasColumnType("datetime");

                entity.Property(e => e.TempDate).HasColumnType("datetime");

                entity.Property(e => e.TransactionPw)
                    .IsRequired()
                    .HasColumnName("TransactionPW")
                    .HasMaxLength(6);

                entity.Property(e => e.ValidThru).IsRequired();

                entity.HasOne(d => d.CcdPerson)
                    .WithMany(p => p.CcdData)
                    .HasForeignKey(d => d.PersonId)
                    .HasConstraintName("FK_dbo.Ccd_Data_dbo.Ccd_Person_PersonId");
            });

            modelBuilder.Entity<CcdDebt>(entity =>
            {
                entity.HasKey(e => e.DebtId);

                entity.ToTable("Ccd_Debt");

                entity.HasIndex(e => e.PersonId)
                    .HasName("IX_PersonID");

                entity.Property(e => e.CurrentAmount).HasDefaultValueSql("((0))");

                entity.Property(e => e.DebtTitle).IsRequired();

                entity.Property(e => e.InterestRate).HasDefaultValueSql("((0))");

                entity.Property(e => e.RepaymentDate).HasColumnType("datetime");

                entity.HasOne(d => d.CcdPerson)
                    .WithMany(p => p.CcdDebt)
                    .HasForeignKey(d => d.PersonId)
                    .HasConstraintName("FK_dbo.Ccd_Debt_dbo.Ccd_Person_PersonId");
            });

            modelBuilder.Entity<CcdPerson>(entity =>
            {
                entity.HasKey(e => e.PersonId);

                entity.ToTable("Ccd_Person");

                entity.Property(e => e.DateOfBirth).HasColumnType("datetime");

                entity.Property(e => e.IdcardNumber).HasColumnName("IDCardNumber");

                entity.Property(e => e.Mobile).IsRequired();

                entity.Property(e => e.PersonName).IsRequired();
            });

            modelBuilder.Entity<CcdPos>(entity =>
            {
                entity.HasKey(e => e.PosId);

                entity.ToTable("Ccd_Pos");

                entity.HasIndex(e => e.PersonId)
                    .HasName("IX_PersonID");

                entity.Property(e => e.PosName).IsRequired();

                entity.HasOne(d => d.CcdPerson)
                    .WithMany(p => p.CcdPos)
                    .HasForeignKey(d => d.PersonId)
                    .HasConstraintName("FK_dbo.Ccd_Pos_dbo.Ccd_Person_PersonId");
            });

            modelBuilder.Entity<CcdRecord>(entity =>
            {
                entity.HasKey(e => e.RecordId);

                entity.ToTable("Ccd_Record");

                entity.HasIndex(e => e.CreditCardId)
                    .HasName("IX_CreditCardID");

                entity.HasIndex(e => e.PosId)
                    .HasName("IX_POSID");

                entity.Property(e => e.RecordDate).HasColumnType("datetime");

                entity.HasOne(d => d.CcdData)
                    .WithMany(p => p.CcdRecord)
                    .HasForeignKey(d => d.CreditCardId)
                    .HasConstraintName("FK_dbo.Ccd_Record_dbo.Ccd_Data_CreditCardId");

                entity.HasOne(d => d.CcdPos)
                    .WithMany(p => p.CcdRecord)
                    .HasForeignKey(d => d.PosId)
                    .HasConstraintName("FK_dbo.Ccd_Record_dbo.Ccd_Pos_PosId");
            });
        }
    }
}
