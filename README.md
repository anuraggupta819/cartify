# Cartify

A full-stack e-commerce platform (Flipkart/Amazon-style), built as a portfolio project — six independently deployable .NET microservices behind a YARP gateway, a React + TypeScript frontend, Google OAuth + JWT auth, inventory-validated checkout, and real Razorpay payment capture (test mode).

**Live**: [cartify-cartify1.vercel.app](https://cartify-cartify1.vercel.app) — browse anonymously, or sign in (Google or the demo admin account) to place a real test-mode order with Razorpay's [published test card](https://razorpay.com/docs/payments/payments/test-card-upi-details/).

```
cartify/
├── backend/    .NET microservices
├── frontend/   React + TypeScript SPA
└── docs/       Cross-cutting design docs and phase roadmap
```

## Architecture

Full microservices (not a modular monolith): each bounded context is an independently deployable service with its own database, fronted by a single API gateway. Every service validates the same shared JWT independently (defense in depth), not just the gateway.

| Service | Responsibility | Ingress |
|---|---|---|
| **Gateway** | YARP reverse proxy, JWT auth enforcement, single public entry point | External — the only backend URL a browser ever reaches |
| **Identity** | Google OAuth (customers) + seeded admin login, issues the shared JWT | Internal |
| **ProductCatalog** | Products & categories, admin-only writes, provisions initial stock on product create | Internal |
| **StockManagement** | Inventory: atomic reserve / release / finalize, admin restock | Internal |
| **OrderManagement** | Order lifecycle, checkout orchestration, abandoned-order sweep | Internal |
| **PaymentProcessing** | Razorpay order creation + payment signature verification | Internal |

Each service follows the same layered structure: `Domain` (entities + invariants) → `Application` (DTOs, service orchestration, abstractions for cross-service calls) → `Infrastructure` (EF Core, repositories, HTTP clients) → `Api` (minimal API endpoints, Swagger, health checks, centralized exception handling).

### Checkout flow

1. Cart lives client-side only (no server-persisted cart) — customer adds items, then checks out.
2. **OrderManagement** fetches each product's current name/price from **ProductCatalog** and reserves stock in **StockManagement** — synchronously, with compensation (releasing already-reserved lines) if any line fails. The order is created in `PendingPayment`.
3. **PaymentProcessing** creates a real Razorpay order and returns it to the frontend, which opens Razorpay Checkout.js.
4. On payment, **PaymentProcessing** verifies the HMAC-SHA256 signature itself (no third-party SDK) and notifies **OrderManagement**, which marks the order `Paid` and finalizes (permanently deducts) stock — or, on failure, cancels the order and releases the reservation.
5. A background sweep in **OrderManagement** cancels and releases stock for any order left in `PendingPayment` for more than 30 minutes (abandoned checkouts).

Cross-service calls (`IStockReservationClient`, `IStockFinalizationClient`, `IOrderPaymentNotifier`, etc.) are interfaces on purpose — synchronous today, but designed as the on-ramp for a future RabbitMQ/MassTransit choreography saga without touching call sites.

### Design principles

- **Single Responsibility** — repositories only persist, services only orchestrate, mapping lives in dedicated extension methods, domain entities own their own invariants (e.g. `Order` refuses to transition out of `PendingPayment` twice).
- **Open/Closed** — cross-cutting concerns (validation-error → HTTP response translation) go through a single exception-handling middleware per service, so adding an endpoint never means re-implementing error handling.
- **Liskov Substitution** — every dependency is used strictly through its interface; nothing downcasts or depends on a concrete implementation.
- **Interface Segregation** — e.g. `IStockReservationClient` (reserve/release) is separate from `IStockFinalizationClient` (finalize) even though both talk to StockManagement, because OrderManagement's checkout path and payment-confirmation path have genuinely different concerns.
- **Dependency Inversion** — `Application` defines every abstraction; `Infrastructure` implements them (EF repositories, typed `HttpClient` wrappers); `Api` wires concrete types via DI only, and never references EF Core or `HttpClient` directly.

Atomic stock updates use EF Core's `ExecuteUpdateAsync` with a conditional `WHERE` clause (not load-then-save), so two concurrent checkouts can't both succeed in reserving the same last unit.

### Auth

Google OAuth (ID-token flow) for customers, seeded username/password for the single admin account — both issue the same first-party JWT (symmetric HMAC, shared signing key across services). The Gateway fails closed: any route not explicitly marked anonymous requires a valid token by default. Service-to-service calls forward the original caller's token; the one background job with no ambient HTTP request (the abandoned-order sweep) mints its own short-lived token off the same shared key.

## Backend

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Either [Docker Desktop](https://www.docker.com/products/docker-desktop) (preferred — matches CI/deployment), or a local [PostgreSQL](https://www.postgresql.org/download/windows/) install for machines where Docker isn't available (e.g. no hardware virtualization support)

### Running locally

Each service needs its own native Postgres database (all under one local Postgres instance, port 5432) and its own port. Migrations apply automatically on startup.

| Service | Local port | Database |
|---|---|---|
| Gateway | 5080 | — |
| Identity | 5213 | `identitydb` |
| ProductCatalog | 5283 | `productcatalogdb` |
| StockManagement | 5285 | `stockmanagementdb` |
| OrderManagement | 5286 | `ordermanagementdb` |
| PaymentProcessing | 5287 | `paymentprocessingdb` |

From each service's `Api` project directory:
```bash
dotnet run --no-launch-profile --urls http://localhost:<port>
```
(If using `--no-launch-profile`, also set `ASPNETCORE_ENVIRONMENT=Development` — that flag skips `launchSettings.json`, which is otherwise what sets it.) PaymentProcessing additionally needs a Razorpay test Key Id/Secret via `dotnet user-secrets` — never committed to config files.

The frontend talks to the Gateway (`http://localhost:5080/api` by default), which routes to everything else — start the Gateway and whichever backend services you're actually exercising.

**With Docker** (preferred where available): `cd backend && docker compose up -d --build`. `Dockerfile`/`docker-compose.yml` stay in the repo either way — validated by CI (Linux runners with full virtualization) and used for the real Azure deployment, even on machines that can't run Docker locally.

### Running backend tests

```bash
cd backend
dotnet test
```

Unit tests always run. Integration tests (ProductCatalog only, currently) use [Testcontainers](https://testcontainers.com/) to spin up a real Postgres instance, so they require Docker — they run in CI even on a machine where Docker isn't available locally.

### Backend layout

```
backend/
├── ECommerceMicroservices.slnx
├── Directory.Build.props            net10.0, Nullable, ImplicitUsings
├── Directory.Packages.props         central package version management
├── docker-compose.yml               local orchestration
├── src/
│   ├── Gateway/ECommerce.Gateway.Api/
│   └── Services/
│       ├── Identity/
│       ├── ProductCatalog/
│       ├── StockManagement/
│       ├── OrderManagement/
│       └── PaymentProcessing/       each: Domain / Application / Infrastructure / Api
└── tests/ProductCatalog/            unit + integration tests
```

## Frontend

React + TypeScript SPA — Vite, Tailwind CSS, TanStack Query, react-router. Product image grid with live stock badges, product detail page, client-side cart, checkout with Razorpay Checkout.js, order history, and an admin stock-management page, alongside the original product/category CRUD.

Layered the same way as the backend: `api/` (typed fetch client, never touched by components) → `hooks/` (TanStack Query wrappers) → `components/` (pure UI, consumes hooks only). `ErrorAlert` surfaces the backend's `ProblemDetails.detail` everywhere.

### Prerequisites

- [Node.js LTS](https://nodejs.org/)
- The backend running locally (see above) — the frontend dev server expects the Gateway at `http://localhost:5080/api` by default (`frontend/.env.local`), plus a Google OAuth Client ID (`VITE_GOOGLE_CLIENT_ID`) for the sign-in button

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
│   ├── api/            typed fetch client + DTOs (products, categories, auth, stock, orders, payments)
│   ├── hooks/           TanStack Query wrappers + useAuth/useCart contexts
│   ├── lib/              formatCurrency, Razorpay Checkout.js loader
│   ├── components/
│   │   ├── layout/        AppShell (header/nav)
│   │   ├── common/        LoadingSpinner, ErrorAlert, ConfirmDialog
│   │   ├── auth/           LoginPage, AdminRoute, RequireAuthRoute
│   │   ├── products/       list/detail/create/edit, ProductCard, ProductForm
│   │   ├── categories/     CategoryQuickCreate
│   │   ├── cart/           CartPage
│   │   ├── checkout/       CheckoutPage (Razorpay integration)
│   │   ├── orders/         OrderHistoryPage
│   │   └── admin/          StockManagementPage
│   └── routes/            react-router config
└── .env.local              VITE_API_BASE_URL, VITE_GOOGLE_CLIENT_ID (gitignored)
```

## Deployment

- **Backend**: Azure Container Apps, scale-to-zero (`--min-replicas 0`) so it stays within the free tier — a few seconds of cold-start after idle. GitHub Container Registry (GHCR) for images, built via a CI matrix strategy across all six services.
- **Database**: [Neon](https://neon.tech) serverless Postgres — one project, one database per service, shared credentials.
- **Frontend**: [Vercel](https://vercel.com), auto-deploys on push to `master`.
- **CI/CD**: `.github/workflows/ci.yml` — backend build/test, frontend build/typecheck, then a matrix build-and-push to GHCR and matrix deploy to Container Apps, all keyed off the same `{service, dockerfile/containerapp}` list.

See [docs/phase-plan.md](docs/phase-plan.md) for the roadmap and what's deliberately deferred.
