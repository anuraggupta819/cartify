# Cartify

A .NET microservices backend for an e-commerce order-processing platform (Flipkart/Amazon-style), built as a portfolio project.

## Architecture

Full microservices (not a modular monolith): each bounded context is an independently deployable service with its own database, communicating synchronously over REST and asynchronously over RabbitMQ (via MassTransit).

| Service | Status | Responsibility |
|---|---|---|
| Catalog | ✅ Phase 1 | Products & categories (CRUD) |
| Ordering | 🔜 Phase 2 | Order lifecycle, order state machine |
| Payment | 🔜 Phase 3 | Simulated payment processing |
| Inventory | 🔜 Phase 3 | Stock reservation |
| Gateway | 🔜 Phase 4 | YARP reverse proxy, single entry point |
| Identity | 🔜 Phase 4 | JWT issuance |

See [docs/phase-plan.md](docs/phase-plan.md) for the full roadmap.

Each service follows a pragmatic layered structure: `Domain` (entities + invariants) → `Application` (DTOs, service orchestration) → `Infrastructure` (EF Core, repositories) → `Api` (minimal API endpoints, Swagger, health checks).

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

## Running locally

```bash
docker compose up -d --build
```

- Catalog API: http://localhost:5101/swagger
- Health check: http://localhost:5101/health

## Running tests

```bash
dotnet test
```

Integration tests use [Testcontainers](https://testcontainers.com/) to spin up a real Postgres instance, so Docker must be running.

## Project layout

```
src/Services/Catalog/   Catalog microservice (Domain/Application/Infrastructure/Api)
tests/Catalog/          Unit and integration tests for the Catalog service
docker-compose.yml       Local orchestration (Postgres + services)
```
