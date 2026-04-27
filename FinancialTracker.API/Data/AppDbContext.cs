using Microsoft.EntityFrameworkCore;
using FinancialTracker.API.Models;

namespace FinancialTracker.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Stock> Stocks => Set<Stock>();
    public DbSet<PriceRecord> PriceRecords => Set<PriceRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Stock>()
            .HasIndex(s => s.Symbol)
            .IsUnique();

        modelBuilder.Entity<PriceRecord>()
            .HasOne(p => p.Stock)
            .WithMany()
            .HasForeignKey(p => p.StockId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
