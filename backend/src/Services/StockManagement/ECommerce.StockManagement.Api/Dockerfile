FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Directory.Build.props Directory.Packages.props ./
COPY src/Services/StockManagement/ECommerce.StockManagement.Domain/ECommerce.StockManagement.Domain.csproj src/Services/StockManagement/ECommerce.StockManagement.Domain/
COPY src/Services/StockManagement/ECommerce.StockManagement.Application/ECommerce.StockManagement.Application.csproj src/Services/StockManagement/ECommerce.StockManagement.Application/
COPY src/Services/StockManagement/ECommerce.StockManagement.Infrastructure/ECommerce.StockManagement.Infrastructure.csproj src/Services/StockManagement/ECommerce.StockManagement.Infrastructure/
COPY src/Services/StockManagement/ECommerce.StockManagement.Api/ECommerce.StockManagement.Api.csproj src/Services/StockManagement/ECommerce.StockManagement.Api/
RUN dotnet restore src/Services/StockManagement/ECommerce.StockManagement.Api/ECommerce.StockManagement.Api.csproj

COPY src/Services/StockManagement/ src/Services/StockManagement/
RUN dotnet publish src/Services/StockManagement/ECommerce.StockManagement.Api/ECommerce.StockManagement.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "ECommerce.StockManagement.Api.dll"]
