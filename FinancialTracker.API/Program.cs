using Microsoft.EntityFrameworkCore;
using FinancialTracker.API.Data;
using FinancialTracker.API.ExternalClients;
using FinancialTracker.API.Repositories;
using FinancialTracker.API.Services;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<IPriceRecordRepository, PriceRecordRepository>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddHttpClient<IAlphaVantageClient, AlphaVantageClient>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();