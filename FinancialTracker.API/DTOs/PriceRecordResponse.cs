namespace FinancialTracker.API.DTOs;

public class PriceRecordResponse
{
    public decimal Price { get; set; }
    public decimal? ChangePercent { get; set; }
    public DateTime FetchedAt { get; set; }
}
