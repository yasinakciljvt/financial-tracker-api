using Moq;
using FinancialTracker.API.DTOs;
using FinancialTracker.API.ExternalClients;
using FinancialTracker.API.Models;
using FinancialTracker.API.Repositories;
using FinancialTracker.API.Services;

namespace FinancialTracker.Tests;

public class StockServiceTests
{
    private readonly Mock<IStockRepository> _stockRepoMock;
    private readonly Mock<IPriceRecordRepository> _priceRepoMock;
private readonly Mock<IQuoteProvider> _quoteProviderMock;
    private readonly StockService _service;

    public StockServiceTests()
    {
        _stockRepoMock = new Mock<IStockRepository>();
        _priceRepoMock = new Mock<IPriceRecordRepository>();
        _quoteProviderMock = new Mock<IQuoteProvider>();
        _service = new StockService(_stockRepoMock.Object, _priceRepoMock.Object, _quoteProviderMock.Object);
    }

    [Fact]
    public async Task AddStockAsync_ShouldReturnStock_WhenSymbolIsNew()
    {
        // Arrange
        _stockRepoMock
            .Setup(r => r.GetBySymbolAsync("AAPL"))
            .ReturnsAsync((Stock?)null);

        var request = new AddStockRequest { Symbol = "AAPL", CompanyName = "Apple Inc." };

        // Act
        var result = await _service.AddStockAsync(request);

        // Assert
        Assert.Equal("AAPL", result.Symbol);
        Assert.Equal("Apple Inc.", result.CompanyName);
        _stockRepoMock.Verify(r => r.AddAsync(It.IsAny<Stock>()), Times.Once);
        _stockRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AddStockAsync_ShouldThrow_WhenSymbolAlreadyExists()
    {
        // Arrange
        var existing = new Stock { Symbol = "AAPL", CompanyName = "Apple Inc." };
        _stockRepoMock
            .Setup(r => r.GetBySymbolAsync("AAPL"))
            .ReturnsAsync(existing);

        var request = new AddStockRequest { Symbol = "AAPL", CompanyName = "Apple Inc." };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.AddStockAsync(request));
    }

    [Fact]
    public async Task DeleteStockAsync_ShouldThrow_WhenStockNotFound()
    {
        // Arrange
        _stockRepoMock
            .Setup(r => r.GetBySymbolAsync("TSLA"))
            .ReturnsAsync((Stock?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteStockAsync("TSLA"));
    }

    [Fact]
    public async Task GetAllStocksAsync_ShouldReturnMappedList()
    {
        // Arrange
        var stocks = new List<Stock>
        {
            new Stock { Id = 1, Symbol = "AAPL", CompanyName = "Apple Inc.", AddedAt = DateTime.UtcNow },
            new Stock { Id = 2, Symbol = "MSFT", CompanyName = "Microsoft", AddedAt = DateTime.UtcNow }
        };

        _stockRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(stocks);
        _priceRepoMock.Setup(r => r.GetLatestByStockIdAsync(It.IsAny<int>())).ReturnsAsync((PriceRecord?)null);

        // Act
        var result = await _service.GetAllStocksAsync();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("AAPL", result[0].Symbol);
        Assert.Equal("MSFT", result[1].Symbol);
    }
}