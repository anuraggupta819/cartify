FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Directory.Build.props Directory.Packages.props ./
COPY src/Services/OrderManagement/ECommerce.OrderManagement.Domain/ECommerce.OrderManagement.Domain.csproj src/Services/OrderManagement/ECommerce.OrderManagement.Domain/
COPY src/Services/OrderManagement/ECommerce.OrderManagement.Application/ECommerce.OrderManagement.Application.csproj src/Services/OrderManagement/ECommerce.OrderManagement.Application/
COPY src/Services/OrderManagement/ECommerce.OrderManagement.Infrastructure/ECommerce.OrderManagement.Infrastructure.csproj src/Services/OrderManagement/ECommerce.OrderManagement.Infrastructure/
COPY src/Services/OrderManagement/ECommerce.OrderManagement.Api/ECommerce.OrderManagement.Api.csproj src/Services/OrderManagement/ECommerce.OrderManagement.Api/
RUN dotnet restore src/Services/OrderManagement/ECommerce.OrderManagement.Api/ECommerce.OrderManagement.Api.csproj

COPY src/Services/OrderManagement/ src/Services/OrderManagement/
RUN dotnet publish src/Services/OrderManagement/ECommerce.OrderManagement.Api/ECommerce.OrderManagement.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "ECommerce.OrderManagement.Api.dll"]
