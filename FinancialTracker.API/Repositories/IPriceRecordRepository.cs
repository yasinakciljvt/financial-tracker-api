using FinancialTracker.API.Models;

namespace FinancialTracker.API.Repositories;

public interface IPriceRecordRepository
{
    Task<List<PriceRecord>> GetByStockIdAsync(int stockId);
    Task<PriceRecord?> GetLatestByStockIdAsync(int stockId);
    Task AddAsync(PriceRecord record);
    Task SaveChangesAsync();
}