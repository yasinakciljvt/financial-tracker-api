using Microsoft.EntityFrameworkCore;
using FinancialTracker.API.Data;
using FinancialTracker.API.Models;

namespace FinancialTracker.API.Repositories;

public class PriceRecordRepository : IPriceRecordRepository
{
    private readonly AppDbContext _context;

    public PriceRecordRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<PriceRecord>> GetByStockIdAsync(int stockId)
    {
        return await _context.PriceRecords
            .Where(p => p.StockId == stockId)
            .OrderByDescending(p => p.FetchedAt)
            .ToListAsync();
    }

    public async Task<PriceRecord?> GetLatestByStockIdAsync(int stockId)
    {
        return await _context.PriceRecords
            .Where(p => p.StockId == stockId)
            .OrderByDescending(p => p.FetchedAt)
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(PriceRecord record)
    {
        await _context.PriceRecords.AddAsync(record);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}