using FinancialTracker.API.DTOs;

namespace FinancialTracker.API.Services;

public interface IStockService
{
    Task<List<StockResponse>> GetAllStocksAsync();
    Task<StockResponse> AddStockAsync(AddStockRequest request);
    Task<PriceRecordResponse> RefreshPriceAsync(string symbol);
    Task DeleteStockAsync(string symbol);
    Task<List<StockResponse>> GetTopGainerStocksAsync(int count);
}