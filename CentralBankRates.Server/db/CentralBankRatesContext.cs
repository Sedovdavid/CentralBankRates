using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CentralBankRates.Server.db;

public partial class CentralBankRatesContext : DbContext
{
    public CentralBankRatesContext()
    {
    }

    public CentralBankRatesContext(DbContextOptions<CentralBankRatesContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CurrenciesCatalog> CurrenciesCatalogs { get; set; }

    public virtual DbSet<RateOnDate> RateOnDates { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-0SSEBUM;Database=central_bank_rates;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CurrenciesCatalog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("currencies_catalog_pk");

            entity.ToTable("currencies_catalog");

            entity.HasIndex(e => e.IsoCharCode, "currencies_catalog_iso_char_code");

            entity.Property(e => e.Id)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("id");
            entity.Property(e => e.Denomination).HasColumnName("denomination");
            entity.Property(e => e.EngName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("eng_name");
            entity.Property(e => e.IsoCharCode)
                .HasMaxLength(4)
                .IsUnicode(false)
                .HasColumnName("iso_char_code");
            entity.Property(e => e.IsoNumCode).HasColumnName("iso_num_code");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.ParentCode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("parent_code");
        });

        modelBuilder.Entity<RateOnDate>(entity =>
        {
            entity.HasKey(e => new { e.Date, e.CurrencyId }).HasName("rate_on_date_pk");

            entity.ToTable("rate_on_date");

            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.CurrencyId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("currencyId");
            entity.Property(e => e.Rate).HasColumnName("rate");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
