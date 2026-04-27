namespace FinancialTracker.API.DTOs;

public class AddStockRequest
{
    public string Symbol { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
}
