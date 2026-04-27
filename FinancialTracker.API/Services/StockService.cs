using FinancialTracker.API.DTOs;
using FinancialTracker.API.ExternalClients;
using FinancialTracker.API.Models;
using FinancialTracker.API.Repositories;

namespace FinancialTracker.API.Services;

public class StockService : IStockService
{
    private readonly IStockRepository _stockRepo;
    private readonly IPriceRecordRepository _priceRepo;
    private readonly AlphaVantageClient _alphaVantage;

    public StockService(
        IStockRepository stockRepo,
        IPriceRecordRepository priceRepo,
        AlphaVantageClient alphaVantage)
    {
        _stockRepo = stockRepo;
        _priceRepo = priceRepo;
        _alphaVantage = alphaVantage;
    }

    public async Task<List<StockResponse>> GetAllStocksAsync()
    {
        var stocks = await _stockRepo.GetAllAsync();
        var result = new List<StockResponse>();

        foreach (var stock in stocks)
        {
            var latest = await _priceRepo.GetLatestByStockIdAsync(stock.Id);
            result.Add(MapToResponse(stock, latest));
        }

        return result;
    }

    public async Task<StockResponse> AddStockAsync(AddStockRequest request)
    {
        var existing = await _stockRepo.GetBySymbolAsync(request.Symbol);
        if (existing != null)
            throw new InvalidOperationException($"{request.Symbol} is already in your watchlist.");

        var stock = new Stock
        {
            Symbol = request.Symbol.ToUpper(),
            CompanyName = request.CompanyName
        };

        await _stockRepo.AddAsync(stock);
        await _stockRepo.SaveChangesAsync();

        return MapToResponse(stock, null);
    }

    public async Task<PriceRecordResponse> RefreshPriceAsync(string symbol)
    {
        var stock = await _stockRepo.GetBySymbolAsync(symbol);
        if (stock == null)
            throw new KeyNotFoundException($"Stock '{symbol}' not found.");

        var quote = await _alphaVantage.GetQuoteAsync(symbol);
        if (quote == null)
            throw new Exception($"Could not fetch price for '{symbol}' from Alpha Vantage.");

        var record = new PriceRecord
        {
            StockId = stock.Id,
            Price = quote.Value.price,
            ChangePercent = quote.Value.changePercent
        };

        await _priceRepo.AddAsync(record);
        await _priceRepo.SaveChangesAsync();

        return new PriceRecordResponse
        {
            Price = record.Price,
            ChangePercent = record.ChangePercent,
            FetchedAt = record.FetchedAt
        };
    }

    public async Task DeleteStockAsync(string symbol)
    {
        var stock = await _stockRepo.GetBySymbolAsync(symbol);
        if (stock == null)
            throw new KeyNotFoundException($"Stock '{symbol}' not found.");

        await _stockRepo.DeleteAsync(stock);
        await _stockRepo.SaveChangesAsync();
    }

    public async Task<List<StockResponse>> GetTopGainerStocksAsync(int count)
    {
        var stocks = await _stockRepo.GetAllAsync();
        var result = new List<StockResponse>();

        foreach (var stock in stocks)
        {
            var latest = await _priceRepo.GetLatestByStockIdAsync(stock.Id);
            result.Add(MapToResponse(stock, latest));
        }

        return result
            .Where(s => s.LatestChangePercent.HasValue)
            .OrderByDescending(s => s.LatestChangePercent)
            .Take(count)
            .ToList();
    }

    private static StockResponse MapToResponse(Stock stock, PriceRecord? latest)
    {
        return new StockResponse
        {
            Id = stock.Id,
            Symbol = stock.Symbol,
            CompanyName = stock.CompanyName,
            LatestPrice = latest?.Price,
            LatestChangePercent = latest?.ChangePercent,
            AddedAt = stock.AddedAt
        };
    }
}