FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Directory.Build.props Directory.Packages.props ./
COPY src/Services/Catalog/ECommerce.Catalog.Domain/ECommerce.Catalog.Domain.csproj src/Services/Catalog/ECommerce.Catalog.Domain/
COPY src/Services/Catalog/ECommerce.Catalog.Application/ECommerce.Catalog.Application.csproj src/Services/Catalog/ECommerce.Catalog.Application/
COPY src/Services/Catalog/ECommerce.Catalog.Infrastructure/ECommerce.Catalog.Infrastructure.csproj src/Services/Catalog/ECommerce.Catalog.Infrastructure/
COPY src/Services/Catalog/ECommerce.Catalog.Api/ECommerce.Catalog.Api.csproj src/Services/Catalog/ECommerce.Catalog.Api/
RUN dotnet restore src/Services/Catalog/ECommerce.Catalog.Api/ECommerce.Catalog.Api.csproj

COPY src/Services/Catalog/ src/Services/Catalog/
RUN dotnet publish src/Services/Catalog/ECommerce.Catalog.Api/ECommerce.Catalog.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "ECommerce.Catalog.Api.dll"]
