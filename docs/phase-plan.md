# Phase Plan

| Phase | Scope | Status |
|---|---|---|
| 1 | Solution scaffold + ProductCatalog service, Postgres, Swagger, health check, Dockerfile, docker-compose, unit + integration tests, basic CI, SOLID-aligned design | ✅ Done |
| 2 | Frontend (React + TypeScript SPA) — ProductCatalog CRUD vertical slice | ✅ Done |
| 3 | Auth: Identity service (Google OAuth + admin login, shared JWT) + Gateway (YARP), ProductCatalog cut over to internal-only ingress behind it | ✅ Done |
| 4 | StockManagement (atomic reserve/release/finalize), OrderManagement (checkout, order lifecycle, abandoned-order sweep), PaymentProcessing (Razorpay order creation + signature verification), product images, cart/checkout/order-history/admin-stock frontend | ✅ Done |
| 5 | CI/CD: GitHub Actions build/test, GHCR image builds via a matrix strategy, Azure Container Apps deploy | ✅ Done |

**Deferred, not dropped:**

- **RabbitMQ/MassTransit choreography saga** for payment confirmation — originally planned as the interview-story centerpiece. The current design uses synchronous calls behind interfaces (`IStockReservationClient`, `IStockFinalizationClient`, `IOrderPaymentNotifier`) specifically so swapping a direct call for a published event later is a localized change, not a rewrite.
- **RS256 + JWKS** for JWT signing — currently symmetric HMAC with a shared secret across services, which is simpler but means every service trusts the same key rather than only verifying against a public key.
- Refresh-token rotation (JWTs are currently long-lived, no refresh flow).
- Razorpay webhook as reliability redundancy alongside the frontend-submitted signature verification (covers the case where the browser never calls back but the payment succeeded).
- Automated test suites (unit/integration) for Identity, Gateway, StockManagement, OrderManagement, and PaymentProcessing — only ProductCatalog has them today.
- Admin visibility into *other users'* orders (order history is currently self-service only).

## Design notes

- **Choreography over orchestration** (for the deferred saga): services would react to each other's events independently rather than through a central saga coordinator — simpler to build, harder to trace at scale, a deliberate tradeoff for this project's size.
- **RabbitMQ on Azure**: not a native PaaS offering. Options when the saga migration happens: RabbitMQ as a sidecar container in the same Container Apps environment, or CloudAMQP's free tier.
- **Auth**: a minimal JWT-issuing `Identity.Api` (Google OAuth + seeded admin) rather than full Duende IdentityServer — enough to demonstrate the pattern without licensing/complexity overhead disproportionate to a portfolio project. See the root [README.md](../README.md#auth) for how the JWT is validated and propagated across services.
- **Local dev without Docker**: the dev machine's CPU doesn't expose hardware virtualization extensions to Windows, so Docker Desktop can't run locally — a firmware/hardware limitation, not fixable in software. `Dockerfile`/`docker-compose.yml` stay in the repo as the source of truth — CI (GitHub Actions, Linux runners) builds and validates them, and they're what the real Azure deployment uses. Day-to-day local dev runs every service directly via `dotnet run` against a natively-installed PostgreSQL 16 instance, one database per service. Integration tests (Testcontainers) still require Docker, so they only run in CI on this machine, not locally.
- **Repo layout**: top-level `backend/` and `frontend/` folders keep the two stacks' tooling (MSBuild/NuGet vs. npm) independent — each has its own dependency lockfile, and CI runs them as separate jobs.
- **Stock semantics**: a product with no `Stock` row (e.g. created before StockManagement existed) is treated as zero available, not "unknown" — both in the API (`GET /api/stock/{id}` 404s, callers should treat that as out-of-stock) and the frontend (shows "Out of stock" rather than nothing).
