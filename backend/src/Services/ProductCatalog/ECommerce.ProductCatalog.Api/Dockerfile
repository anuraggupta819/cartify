FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Directory.Build.props Directory.Packages.props ./
COPY src/Services/ProductCatalog/ECommerce.ProductCatalog.Domain/ECommerce.ProductCatalog.Domain.csproj src/Services/ProductCatalog/ECommerce.ProductCatalog.Domain/
COPY src/Services/ProductCatalog/ECommerce.ProductCatalog.Application/ECommerce.ProductCatalog.Application.csproj src/Services/ProductCatalog/ECommerce.ProductCatalog.Application/
COPY src/Services/ProductCatalog/ECommerce.ProductCatalog.Infrastructure/ECommerce.ProductCatalog.Infrastructure.csproj src/Services/ProductCatalog/ECommerce.ProductCatalog.Infrastructure/
COPY src/Services/ProductCatalog/ECommerce.ProductCatalog.Api/ECommerce.ProductCatalog.Api.csproj src/Services/ProductCatalog/ECommerce.ProductCatalog.Api/
RUN dotnet restore src/Services/ProductCatalog/ECommerce.ProductCatalog.Api/ECommerce.ProductCatalog.Api.csproj

COPY src/Services/ProductCatalog/ src/Services/ProductCatalog/
RUN dotnet publish src/Services/ProductCatalog/ECommerce.ProductCatalog.Api/ECommerce.ProductCatalog.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "ECommerce.ProductCatalog.Api.dll"]
