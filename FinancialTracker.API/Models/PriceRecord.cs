namespace FinancialTracker.API.Models;

public class PriceRecord
{
    public int Id { get; set; }
    public int StockId { get; set; }
    public Stock Stock { get; set; } = null!;
    public decimal Price { get; set; }
    public decimal? ChangePercent { get; set; }
    public DateTime FetchedAt { get; set; } = DateTime.UtcNow;
}
