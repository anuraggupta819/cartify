FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Directory.Build.props Directory.Packages.props ./
COPY src/Services/PaymentProcessing/ECommerce.PaymentProcessing.Domain/ECommerce.PaymentProcessing.Domain.csproj src/Services/PaymentProcessing/ECommerce.PaymentProcessing.Domain/
COPY src/Services/PaymentProcessing/ECommerce.PaymentProcessing.Application/ECommerce.PaymentProcessing.Application.csproj src/Services/PaymentProcessing/ECommerce.PaymentProcessing.Application/
COPY src/Services/PaymentProcessing/ECommerce.PaymentProcessing.Infrastructure/ECommerce.PaymentProcessing.Infrastructure.csproj src/Services/PaymentProcessing/ECommerce.PaymentProcessing.Infrastructure/
COPY src/Services/PaymentProcessing/ECommerce.PaymentProcessing.Api/ECommerce.PaymentProcessing.Api.csproj src/Services/PaymentProcessing/ECommerce.PaymentProcessing.Api/
RUN dotnet restore src/Services/PaymentProcessing/ECommerce.PaymentProcessing.Api/ECommerce.PaymentProcessing.Api.csproj

COPY src/Services/PaymentProcessing/ src/Services/PaymentProcessing/
RUN dotnet publish src/Services/PaymentProcessing/ECommerce.PaymentProcessing.Api/ECommerce.PaymentProcessing.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "ECommerce.PaymentProcessing.Api.dll"]
