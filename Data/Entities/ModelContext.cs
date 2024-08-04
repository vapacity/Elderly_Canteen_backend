using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Elderly_Canteen.Data.Entities;

public partial class ModelContext : DbContext
{
    public ModelContext()
    {
    }

    public ModelContext(DbContextOptions<ModelContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseOracle("User Id=ELDERLY_CANTEEN;Password=CANTEEN_PASSWORD;Data Source=124.220.16.200:1521/xe");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("ELDERLY_CANTEEN");

        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Accountid).HasName("SYS_C0010000");

            entity.ToTable("ACCOUNT");

            entity.HasIndex(e => e.Accountname, "IDX_ACCOUNTNAME");

            entity.HasIndex(e => e.Idcard, "UNIQUE_IDCARD").IsUnique();

            entity.HasIndex(e => e.Phonenum, "UNIQUE_PHONENUM").IsUnique();

            entity.Property(e => e.Accountid)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ACCOUNTID");
            entity.Property(e => e.Accountname)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("ACCOUNTNAME");
            entity.Property(e => e.Address)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("ADDRESS");
            entity.Property(e => e.Birthdate)
                .HasColumnType("DATE")
                .HasColumnName("BIRTHDATE");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("GENDER");
            entity.Property(e => e.Idcard)
                .HasMaxLength(18)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("IDCARD");
            entity.Property(e => e.Identity)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("IDENTITY");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("NAME");
            entity.Property(e => e.Password)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("PASSWORD");
            entity.Property(e => e.Phonenum)
                .HasMaxLength(11)
                .IsUnicode(false)
                .HasColumnName("PHONENUM");
            entity.Property(e => e.Portrait)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("PORTRAIT");
            entity.Property(e => e.Verifycode)
                .HasPrecision(6)
                .HasColumnName("VERIFYCODE");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
