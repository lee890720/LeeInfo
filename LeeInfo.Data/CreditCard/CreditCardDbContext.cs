using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LeeInfo.Data.CreditCard
{
    public partial class CreditCardDbContext : DbContext
    {
        public virtual DbSet<CbotSet> CbotSet { get; set; }
        public virtual DbSet<CreditCardAccount> CreditCardAccount { get; set; }
        public virtual DbSet<CreditCardBill> CreditCardBill { get; set; }
        public virtual DbSet<CreditCardPos> CreditCardPos { get; set; }
        public virtual DbSet<CreditCardRecord> CreditCardRecord { get; set; }
        public virtual DbSet<Person> Person { get; set; }
        public virtual DbSet<PersonDebt> PersonDebt { get; set; }

        public CreditCardDbContext(DbContextOptions<CreditCardDbContext> options)
    : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CbotSet>(entity =>
            {
                entity.ToTable("CBotSet");

                entity.HasIndex(e => e.Symbol)
                    .HasName("UQ__CBotSet__DF7EEB815772F790")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Alike).HasColumnName("alike");

                entity.Property(e => e.Averageperiods).HasColumnName("averageperiods");

                entity.Property(e => e.Breakfirst).HasColumnName("breakfirst");

                entity.Property(e => e.Brk).HasColumnName("brk");

                entity.Property(e => e.Ca).HasColumnName("ca");

                entity.Property(e => e.Cr).HasColumnName("cr");

                entity.Property(e => e.Distance).HasColumnName("distance");

                entity.Property(e => e.Initvolume).HasColumnName("initvolume");

                entity.Property(e => e.Isbreak).HasColumnName("isbreak");

                entity.Property(e => e.Istrade).HasColumnName("istrade");

                entity.Property(e => e.Magnify).HasColumnName("magnify");

                entity.Property(e => e.Resultperiods).HasColumnName("resultperiods");

                entity.Property(e => e.Sa).HasColumnName("sa");

                entity.Property(e => e.Signal)
                    .HasColumnName("signal")
                    .HasMaxLength(50);

                entity.Property(e => e.Slippage).HasColumnName("slippage");

                entity.Property(e => e.Sr).HasColumnName("sr");

                entity.Property(e => e.Sub).HasColumnName("sub");

                entity.Property(e => e.Symbol)
                    .IsRequired()
                    .HasColumnName("symbol")
                    .HasMaxLength(50);

                entity.Property(e => e.Tmr).HasColumnName("tmr");
            });

            modelBuilder.Entity<CreditCardAccount>(entity =>
            {
                entity.HasKey(e => e.CreditCardId);

                entity.HasIndex(e => e.PersonId)
                    .HasName("IX_PersonID");

                entity.Property(e => e.CreditCardId).HasColumnName("CreditCardID");

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

                entity.Property(e => e.PersonId).HasColumnName("PersonID");

                entity.Property(e => e.PrePayment).HasDefaultValueSql("((0))");

                entity.Property(e => e.RepaymentDate).HasColumnType("datetime");

                entity.Property(e => e.TempDate).HasColumnType("datetime");

                entity.Property(e => e.TransactionPw)
                    .IsRequired()
                    .HasColumnName("TransactionPW")
                    .HasMaxLength(6);

                entity.Property(e => e.ValidThru).IsRequired();

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.CreditCardAccount)
                    .HasForeignKey(d => d.PersonId)
                    .HasConstraintName("FK_dbo.CreditCardAccount_dbo.Person_PersonID");
            });

            modelBuilder.Entity<CreditCardBill>(entity =>
            {
                entity.HasKey(e => e.BillId);

                entity.HasIndex(e => e.CreditCardId)
                    .HasName("IX_CreditCardID");

                entity.Property(e => e.BillId).HasColumnName("BillID");

                entity.Property(e => e.BillDate).HasColumnType("datetime");

                entity.Property(e => e.CreditCardId).HasColumnName("CreditCardID");

                entity.HasOne(d => d.CreditCard)
                    .WithMany(p => p.CreditCardBill)
                    .HasForeignKey(d => d.CreditCardId)
                    .HasConstraintName("FK_dbo.CreditCardBill_dbo.CreditCardAccount_CreditCardID");
            });

            modelBuilder.Entity<CreditCardPos>(entity =>
            {
                entity.HasKey(e => e.Posid);

                entity.HasIndex(e => e.PersonId)
                    .HasName("IX_PersonID");

                entity.Property(e => e.Posid).HasColumnName("POSID");

                entity.Property(e => e.PersonId).HasColumnName("PersonID");

                entity.Property(e => e.Posname)
                    .IsRequired()
                    .HasColumnName("POSName");

                entity.Property(e => e.Posnote).HasColumnName("POSNote");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.CreditCardPos)
                    .HasForeignKey(d => d.PersonId)
                    .HasConstraintName("FK_dbo.CreditCardPOS_dbo.Person_PersonID");
            });

            modelBuilder.Entity<CreditCardRecord>(entity =>
            {
                entity.HasKey(e => e.RecordId);

                entity.HasIndex(e => e.CreditCardId)
                    .HasName("IX_CreditCardID");

                entity.HasIndex(e => e.Posid)
                    .HasName("IX_POSID");

                entity.Property(e => e.RecordId).HasColumnName("RecordID");

                entity.Property(e => e.CreditCardId).HasColumnName("CreditCardID");

                entity.Property(e => e.Posid).HasColumnName("POSID");

                entity.Property(e => e.RecordDate).HasColumnType("datetime");

                entity.HasOne(d => d.CreditCard)
                    .WithMany(p => p.CreditCardRecord)
                    .HasForeignKey(d => d.CreditCardId)
                    .HasConstraintName("FK_dbo.CreditCardRecord_dbo.CreditCardAccount_CreditCardID");

                entity.HasOne(d => d.Pos)
                    .WithMany(p => p.CreditCardRecord)
                    .HasForeignKey(d => d.Posid)
                    .HasConstraintName("FK_dbo.CreditCardRecord_dbo.CreditCardPOS_POSID");
            });

            modelBuilder.Entity<Person>(entity =>
            {
                entity.Property(e => e.PersonId).HasColumnName("PersonID");

                entity.Property(e => e.DateOfBirth).HasColumnType("datetime");

                entity.Property(e => e.IdcardNumber).HasColumnName("IDCardNumber");

                entity.Property(e => e.Mobile).IsRequired();

                entity.Property(e => e.PersonName).IsRequired();
            });

            modelBuilder.Entity<PersonDebt>(entity =>
            {
                entity.HasKey(e => e.DebtId);

                entity.HasIndex(e => e.PersonId)
                    .HasName("IX_PersonID");

                entity.Property(e => e.DebtId).HasColumnName("DebtID");

                entity.Property(e => e.CurrentAmount).HasDefaultValueSql("((0))");

                entity.Property(e => e.DebtTitle).IsRequired();

                entity.Property(e => e.InterestRate).HasDefaultValueSql("((0))");

                entity.Property(e => e.PersonId).HasColumnName("PersonID");

                entity.Property(e => e.RepaymentDate).HasColumnType("datetime");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.PersonDebt)
                    .HasForeignKey(d => d.PersonId)
                    .HasConstraintName("FK_dbo.PersonDebt_dbo.Person_PersonID");
            });
        }
    }
}
