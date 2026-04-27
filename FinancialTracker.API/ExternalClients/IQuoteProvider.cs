namespace FinancialTracker.API.ExternalClients;

// Strategy Pattern: fiyat verisi çekme davranışını soyutlar.
// Farklı API sağlayıcıları (Alpha Vantage, Finnhub vb.) bu interface'i implemente eder.
// StockService hangi sağlayıcının kullanıldığını bilmek zorunda değildir.
public interface IQuoteProvider
{
    Task<(decimal price, decimal changePercent)?> GetQuoteAsync(string symbol);
}