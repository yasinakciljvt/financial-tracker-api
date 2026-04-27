FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY FinancialTracker.sln .
COPY FinancialTracker.API/FinancialTracker.API.csproj FinancialTracker.API/
COPY FinancialTracker.Tests/FinancialTracker.Tests.csproj FinancialTracker.Tests/

RUN dotnet restore

COPY . .

RUN dotnet publish FinancialTracker.API/FinancialTracker.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "FinancialTracker.API.dll"]