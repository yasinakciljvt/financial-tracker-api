using Microsoft.EntityFrameworkCore;
using FinancialTracker.API.Data;
using FinancialTracker.API.Models;
using FinancialTracker.API.Repositories;


namespace FinancialTracker.API.Repositories;

public class StockRepository : IStockRepository
{
    private readonly AppDbContext _context;

    public StockRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Stock>> GetAllAsync()
    {
        return await _context.Stocks.ToListAsync();
    }
public Task DeleteAsync(Stock stock)
{
    _context.Stocks.Remove(stock);
    return Task.CompletedTask;
}

    public async Task<Stock?> GetBySymbolAsync(string symbol)
    {
        return await _context.Stocks
            .FirstOrDefaultAsync(s => s.Symbol.ToUpper() == symbol.ToUpper());
    }

    public async Task<Stock?> GetByIdAsync(int id)
    {
        return await _context.Stocks.FindAsync(id);
    }

    public async Task AddAsync(Stock stock)
    {
        await _context.Stocks.AddAsync(stock);
    }


    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}