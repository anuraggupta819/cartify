# Cartify

A full-stack e-commerce order-processing platform (Flipkart/Amazon-style), built as a portfolio project — .NET microservices backend, React + TypeScript frontend.

```
cartify/
├── backend/    .NET microservices
├── frontend/   React + TypeScript SPA
└── docs/       Cross-cutting design docs and phase roadmap
```

## Backend

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

### Backend design principles

- **Single Responsibility** — repositories only persist, services only orchestrate, mapping lives in dedicated extension methods, domain entities own their own invariants.
- **Open/Closed** — cross-cutting concerns (validation-error → HTTP response translation) are handled by a single exception-handling middleware, so adding a new endpoint never requires re-implementing error handling.
- **Liskov Substitution** — every repository is used strictly through its interface (`IProductRepository`, `ICategoryRepository`); nothing downcasts or depends on a concrete implementation.
- **Interface Segregation** — `IUnitOfWork` is separate from the repository interfaces, so a consumer that only needs to persist doesn't have to depend on query methods it never calls.
- **Dependency Inversion** — `Application` defines the abstractions (`IProductRepository`, `IUnitOfWork`); `Infrastructure` implements them; `Api` wires concrete types via DI only, and never references EF Core directly.

### Backend prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Either [Docker Desktop](https://www.docker.com/products/docker-desktop) (preferred — matches CI/deployment), or a local [PostgreSQL](https://www.postgresql.org/download/windows/) install for machines where Docker isn't available (e.g. no hardware virtualization support)

### Running the backend

**With Docker** (preferred):
```bash
cd backend
docker compose up -d --build
```
- ProductCatalog API: http://localhost:5101/swagger
- Health check: http://localhost:5101/health

**Without Docker** (native Postgres):
1. Install PostgreSQL and create a `productcatalogdb` database (default port 5432, user `postgres`).
2. `backend/src/Services/ProductCatalog/ECommerce.ProductCatalog.Api/appsettings.Development.json` already points at `localhost:5432` — adjust credentials there if yours differ.
3. From `backend/src/Services/ProductCatalog/ECommerce.ProductCatalog.Api`, run:
   ```bash
   dotnet run
   ```
   Migrations apply automatically on startup.
- ProductCatalog API: http://localhost:5283/swagger (port from `launchSettings.json`; opens automatically)

The `Dockerfile`/`docker-compose.yml` stay in the repo either way — they're validated by CI (which runs on Linux runners with full virtualization) even on machines that can't run Docker locally, and they're what a real deployment (Azure Container Apps) would use.

### Running backend tests

```bash
cd backend
dotnet test
```

Unit tests always run. Integration tests use [Testcontainers](https://testcontainers.com/) to spin up a real Postgres instance in a container, so they require Docker specifically — they'll pass in CI even on a machine where Docker isn't available locally.

### Backend layout

```
backend/
├── ECommerceMicroservices.slnx
├── Directory.Build.props          net10.0, Nullable, ImplicitUsings
├── Directory.Packages.props       central package version management
├── docker-compose.yml             local orchestration (Postgres + services)
├── src/Services/ProductCatalog/   ProductCatalog microservice (Domain/Application/Infrastructure/Api)
└── tests/ProductCatalog/          Unit and integration tests for the ProductCatalog service
```

## Frontend

React + TypeScript SPA against the backend API — Vite, Tailwind CSS, TanStack Query, react-router. Currently covers the ProductCatalog vertical slice: product list (paginated), create/edit/delete, inline category creation.

Layered the same way as the backend: `api/` (typed fetch client, never touched by components) → `hooks/` (TanStack Query wrappers) → `components/` (pure UI, consumes hooks only). `ErrorAlert` surfaces the backend's `ProblemDetails.detail` everywhere, matching the API's centralized exception handling.

### Frontend prerequisites

- [Node.js LTS](https://nodejs.org/)
- The backend running locally (see above) — the frontend dev server expects the API at `http://localhost:5283/api` by default (`frontend/.env.local`)

### Running the frontend

```bash
cd frontend
npm install
npm run dev
```
Opens at http://localhost:5173.

### Frontend layout

```
frontend/
├── src/
│   ├── api/           typed fetch client + DTO types (ProductDto, CategoryDto, ApiError, ...)
│   ├── hooks/          TanStack Query wrappers (useProducts, useCategories)
│   ├── components/
│   │   ├── layout/      AppShell (header/nav)
│   │   ├── common/      LoadingSpinner, ErrorAlert, ConfirmDialog
│   │   ├── products/    ProductListPage, ProductForm, Create/Edit pages, DeleteProductButton
│   │   └── categories/  CategoryQuickCreate
│   └── routes/         react-router config
└── .env.local           VITE_API_BASE_URL (gitignored)
```

## Running both together

1. `cd backend/src/Services/ProductCatalog/ECommerce.ProductCatalog.Api && dotnet run`
2. `cd frontend && npm run dev`
3. Open http://localhost:5173
