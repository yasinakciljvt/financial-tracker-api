using System.Text.Json;

namespace FinancialTracker.API.ExternalClients;

// Strategy Pattern: Alpha Vantage bu interface'in bir implementasyonudur.
public class AlphaVantageClient : IQuoteProvider
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _baseUrl;

    public AlphaVantageClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["AlphaVantage:ApiKey"] ?? throw new InvalidOperationException("AlphaVantage API key is not configured.");
        _baseUrl = configuration["AlphaVantage:BaseUrl"] ?? "https://www.alphavantage.co/query";
    }

    public async Task<(decimal price, decimal changePercent)?> GetQuoteAsync(string symbol)
    {
        var url = $"{_baseUrl}?function=GLOBAL_QUOTE&symbol={symbol}&apikey={_apiKey}";

        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
            return null;

        var content = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(content);

        if (!json.RootElement.TryGetProperty("Global Quote", out var quote))
            return null;

        if (!quote.TryGetProperty("05. price", out var priceEl) ||
            !quote.TryGetProperty("10. change percent", out var changeEl))
            return null;

        var priceStr = priceEl.GetString() ?? "0";
        var changeStr = changeEl.GetString()?.Replace("%", "") ?? "0";

        if (!decimal.TryParse(priceStr, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var price))
            return null;

        decimal.TryParse(changeStr, System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture, out var changePercent);

        return (price, changePercent);
    }
}