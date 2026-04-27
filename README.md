# Financial Tracker

Stock watchlist API built with .NET 8. Tracks prices via Alpha Vantage and exposes a simple REST interface for managing a personal watchlist.

## Stack

- .NET 8 Web API
- SQLite + Entity Framework Core
- Alpha Vantage (free tier)
- xUnit + Moq

## Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/stocks` | List all stocks with latest price |
| POST | `/api/stocks` | Add a stock to the watchlist |
| POST | `/api/stocks/{symbol}/refresh` | Fetch latest price from Alpha Vantage |
| DELETE | `/api/stocks/{symbol}` | Remove a stock |
| GET | `/api/stocks/analytics/top-gainers?count=5` | Top N stocks by daily change % |

## Design Patterns

**Repository Pattern** (`IStockRepository`, `IPriceRecordRepository`) — keeps the service layer decoupled from EF Core. Easier to test, easier to swap storage later.

**Strategy Pattern** (`IQuoteProvider`, `AlphaVantageClient`) — price fetching is behind an interface. Switching to a different API provider means writing one new class, nothing else changes.

## Setup

```bash
git clone <https://github.com/yasinakciljvt/financial-tracker-api.git>
cd financial-tracker-api

export AlphaVantage__ApiKey="your_key"
cd FinancialTracker.API
dotnet run
```

Swagger: `http://localhost:5246/swagger`

## Docker

```bash
cp .env.example .env
# add your key to .env
docker compose up --build
```

Swagger: `http://localhost:8080/swagger`

## Tests

```bash
dotnet test
```

## Notes

- Alpha Vantage free tier is 25 req/day. If refresh returns no data, you've likely hit the limit.
- No auth — designed as an internal tool.
- Each refresh appends a new price record, old ones are kept.
