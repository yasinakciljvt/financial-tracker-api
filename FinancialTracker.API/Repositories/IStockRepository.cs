using FinancialTracker.API.Models;

namespace FinancialTracker.API.Repositories;

// Repository Pattern: veritabanı işlemlerini soyutlar,
// service katmanı EF Core'a doğrudan bağımlı olmaz.
public interface IStockRepository
{
    Task<List<Stock>> GetAllAsync();
    Task<Stock?> GetBySymbolAsync(string symbol);
    Task<Stock?> GetByIdAsync(int id);
    Task AddAsync(Stock stock);
    Task DeleteAsync(Stock stock);
    Task SaveChangesAsync();
}