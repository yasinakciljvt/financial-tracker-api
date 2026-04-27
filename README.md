# Financial Tracker

A simple stock watchlist API built with .NET 8. Fetches real-time quotes from Alpha Vantage, stores them locally, and exposes a REST API for managing a personal watchlist.

---

## Tech Stack

- **Framework:** .NET 8 Web API
- **Database:** SQLite via Entity Framework Core
- **External API:** [Alpha Vantage](https://www.alphavantage.co) — free tier, 25 req/day
- **Testing:** xUnit + Moq
- **Docs:** Swagger / OpenAPI

---

## Project Structure
FinancialTracker.API/
├── Controllers/       # HTTP layer — no business logic here
├── Services/          # Business logic
├── Repositories/      # Data access (Repository pattern)
├── Models/            # EF Core entities
├── DTOs/              # Request / response objects
├── Data/              # DbContext and migrations
└── ExternalClients/   # Alpha Vantage HTTP client

---

## Design Patterns

**Repository Pattern** — `IStockRepository`, `IPriceRecordRepository`

The service layer never touches `DbContext` directly. This keeps business logic decoupled from EF Core, makes unit testing straightforward (repositories are mocked via interfaces), and means the storage layer can be swapped without touching service code.

**Strategy Pattern** — `IQuoteProvider`, `AlphaVantageClient`

Price fetching behavior is abstracted behind `IQuoteProvider`. `StockService` has no knowledge of which API provider is being used. Swapping from Alpha Vantage to Finnhub (or any other source) requires only a new implementation — no changes to business logic or controllers. This also makes the service fully testable without real HTTP calls.
---

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/stocks` | List all tracked stocks with latest price |
| POST | `/api/stocks` | Add a stock to the watchlist |
| POST | `/api/stocks/{symbol}/refresh` | Fetch latest price from Alpha Vantage |
| DELETE | `/api/stocks/{symbol}` | Remove a stock |
| GET | `/api/stocks/analytics/top-gainers?count=5` | Top N stocks by daily change % |

---

## Setup & Run

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- A free [Alpha Vantage API key](https://www.alphavantage.co/support/#api-key)

### Run locally

```bash
git clone <repo-url>
cd FinancialTracker

export AlphaVantage__ApiKey="your_api_key_here"
cd FinancialTracker.API
dotnet run
```

Swagger UI: `http://localhost:5246/swagger`

### Run with Docker

```bash
cp .env.example .env
# Edit .env and set your API key

docker compose up --build
```

Swagger UI: `http://localhost:8080/swagger`

---

## Environment Variables

| Variable | Description |
|----------|-------------|
| `AlphaVantage__ApiKey` | Your Alpha Vantage API key |
| `ConnectionStrings__DefaultConnection` | SQLite path (default: `financial_tracker.db`) |

Never commit your API key. The `appsettings.json` contains only a placeholder.

---

## Running Tests

```bash
dotnet test
```

4 unit tests covering the service layer — add stock, duplicate detection, delete not found, and list mapping.

---

## Trade-offs & Notes

- Alpha Vantage free tier allows 25 requests/day. Rapid refreshes will return empty price data — this is handled gracefully with a `400` response.
- No authentication — this is intended as an internal tool, not a public API.
- Price history is append-only. Each `/refresh` call adds a new record; old records are kept for potential future analytics.