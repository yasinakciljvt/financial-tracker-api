namespace FinancialTracker.API.ExternalClients;

public interface IAlphaVantageClient
{
    Task<(decimal price, decimal changePercent)?> GetQuoteAsync(string symbol);
}