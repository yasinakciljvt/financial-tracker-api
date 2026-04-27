namespace FinancialTracker.API.DTOs;

public class StockResponse
{
    public int Id { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public decimal? LatestPrice { get; set; }
    public decimal? LatestChangePercent { get; set; }
    public DateTime AddedAt { get; set; }
}
