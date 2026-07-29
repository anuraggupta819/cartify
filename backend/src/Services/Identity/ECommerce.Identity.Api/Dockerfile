FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Directory.Build.props Directory.Packages.props ./
COPY src/Services/Identity/ECommerce.Identity.Domain/ECommerce.Identity.Domain.csproj src/Services/Identity/ECommerce.Identity.Domain/
COPY src/Services/Identity/ECommerce.Identity.Application/ECommerce.Identity.Application.csproj src/Services/Identity/ECommerce.Identity.Application/
COPY src/Services/Identity/ECommerce.Identity.Infrastructure/ECommerce.Identity.Infrastructure.csproj src/Services/Identity/ECommerce.Identity.Infrastructure/
COPY src/Services/Identity/ECommerce.Identity.Api/ECommerce.Identity.Api.csproj src/Services/Identity/ECommerce.Identity.Api/
RUN dotnet restore src/Services/Identity/ECommerce.Identity.Api/ECommerce.Identity.Api.csproj

COPY src/Services/Identity/ src/Services/Identity/
RUN dotnet publish src/Services/Identity/ECommerce.Identity.Api/ECommerce.Identity.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "ECommerce.Identity.Api.dll"]
