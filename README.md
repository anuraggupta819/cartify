# Cartify

A .NET microservices backend for an e-commerce order-processing platform (Flipkart/Amazon-style), built as a portfolio project.

## Architecture

Full microservices (not a modular monolith): each bounded context is an independently deployable service with its own database, communicating synchronously over REST and asynchronously over RabbitMQ (via MassTransit).

| Service | Status | Responsibility |
|---|---|---|
| ProductCatalog | ✅ Phase 1 | Products & categories (CRUD) |
| OrderManagement | 🔜 Phase 2 | Order lifecycle, order state machine |
| PaymentProcessing | 🔜 Phase 3 | Simulated payment processing |
| StockManagement | 🔜 Phase 3 | Stock reservation |
| Gateway | 🔜 Phase 4 | YARP reverse proxy, single entry point |
| Identity | 🔜 Phase 4 | JWT issuance |

See [docs/phase-plan.md](docs/phase-plan.md) for the full roadmap.

Each service follows a pragmatic layered structure: `Domain` (entities + invariants) → `Application` (DTOs, service orchestration, Unit of Work) → `Infrastructure` (EF Core, repositories) → `Api` (minimal API endpoints, Swagger, health checks, centralized exception handling).

### Design principles

- **Single Responsibility** — repositories only persist, services only orchestrate, mapping lives in dedicated extension methods, domain entities own their own invariants.
- **Open/Closed** — cross-cutting concerns (validation-error → HTTP response translation) are handled by a single exception-handling middleware, so adding a new endpoint never requires re-implementing error handling.
- **Liskov Substitution** — every repository is used strictly through its interface (`IProductRepository`, `ICategoryRepository`); nothing downcasts or depends on a concrete implementation.
- **Interface Segregation** — `IUnitOfWork` is separate from the repository interfaces, so a consumer that only needs to persist doesn't have to depend on query methods it never calls.
- **Dependency Inversion** — `Application` defines the abstractions (`IProductRepository`, `IUnitOfWork`); `Infrastructure` implements them; `Api` wires concrete types via DI only, and never references EF Core directly.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Either [Docker Desktop](https://www.docker.com/products/docker-desktop) (preferred — matches CI/deployment), or a local [PostgreSQL](https://www.postgresql.org/download/windows/) install for machines where Docker isn't available (e.g. no hardware virtualization support)

## Running locally

**With Docker** (preferred):
```bash
docker compose up -d --build
```
- ProductCatalog API: http://localhost:5101/swagger
- Health check: http://localhost:5101/health

**Without Docker** (native Postgres):
1. Install PostgreSQL and create a `productcatalogdb` database (default port 5432, user `postgres`).
2. `appsettings.Development.json` already points at `localhost:5432` — adjust credentials there if yours differ.
3. From `src/Services/ProductCatalog/ECommerce.ProductCatalog.Api`, run:
   ```bash
   dotnet run
   ```
   Migrations apply automatically on startup.
- ProductCatalog API: http://localhost:5283/swagger (port from `launchSettings.json`; opens automatically)

The `Dockerfile`/`docker-compose.yml` stay in the repo either way — they're validated by CI (which runs on Linux runners with full virtualization) even on machines that can't run Docker locally, and they're what a real deployment (Azure Container Apps) would use.

## Running tests

```bash
dotnet test
```

Unit tests always run. Integration tests use [Testcontainers](https://testcontainers.com/) to spin up a real Postgres instance in a container, so they require Docker specifically — they'll pass in CI even on a machine where Docker isn't available locally.

## Project layout

```
src/Services/ProductCatalog/   ProductCatalog microservice (Domain/Application/Infrastructure/Api)
tests/ProductCatalog/          Unit and integration tests for the ProductCatalog service
docker-compose.yml              Local orchestration (Postgres + services)
```
