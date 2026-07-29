FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Directory.Build.props Directory.Packages.props ./
COPY src/Gateway/ECommerce.Gateway.Api/ECommerce.Gateway.Api.csproj src/Gateway/ECommerce.Gateway.Api/
RUN dotnet restore src/Gateway/ECommerce.Gateway.Api/ECommerce.Gateway.Api.csproj

COPY src/Gateway/ src/Gateway/
RUN dotnet publish src/Gateway/ECommerce.Gateway.Api/ECommerce.Gateway.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "ECommerce.Gateway.Api.dll"]
