using Microsoft.AspNetCore.Mvc;
using FinancialTracker.API.DTOs;
using FinancialTracker.API.Services;

namespace FinancialTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StocksController : ControllerBase
{
    private readonly IStockService _stockService;

    public StocksController(IStockService stockService)
    {
        _stockService = stockService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var stocks = await _stockService.GetAllStocksAsync();
        return Ok(stocks);
    }

    [HttpPost]
    public async Task<IActionResult> AddStock([FromBody] AddStockRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Symbol))
            return BadRequest("Symbol is required.");

        try
        {
            var stock = await _stockService.AddStockAsync(request);
            return CreatedAtAction(nameof(GetAll), new { }, stock);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpPost("{symbol}/refresh")]
    public async Task<IActionResult> RefreshPrice(string symbol)
    {
        try
        {
            var price = await _stockService.RefreshPriceAsync(symbol);
            return Ok(price);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{symbol}")]
    public async Task<IActionResult> DeleteStock(string symbol)
    {
        try
        {
            await _stockService.DeleteStockAsync(symbol);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("analytics/top-gainers")]
    public async Task<IActionResult> GetTopGainers([FromQuery] int count = 5)
    {
        if (count <= 0)
            return BadRequest("Count must be greater than zero.");

        var stocks = await _stockService.GetTopGainerStocksAsync(count);
        return Ok(stocks);
    }
}