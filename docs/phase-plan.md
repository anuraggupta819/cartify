# Phase Plan

| Phase | Scope | Status |
|---|---|---|
| 1 | Solution scaffold + ProductCatalog service (Domain/Application/Infrastructure/Api), Postgres, Swagger, health check, Dockerfile, docker-compose, unit + integration tests, basic CI, SOLID-aligned design (Unit of Work, centralized exception handling, mapping extraction) | ✅ Done |
| 2 | OrderManagement service (Order aggregate + state machine), `Shared.Contracts` lib, MassTransit + RabbitMQ added to compose, OrderManagement publishes `OrderCreated` | 🔜 Next |
| 3 | PaymentProcessing + StockManagement services consume `OrderCreated`, simulate processing, publish success/failure events; OrderManagement implements saga completion **and compensation** (e.g. release stock if payment fails) | 🔜 |
| 3.5 (optional) | Notification service — fan-out consumer of `OrderConfirmed` | 🔜 Stretch |
| 4 | API Gateway (YARP) as single entry point; minimal JWT-issuing `Identity.Api`; services validate bearer tokens | 🔜 |
| 5 | Serilog + Seq, correlation-ID propagation across services, health checks wired into compose | 🔜 |
| 6 | GitHub Actions CI/CD (build/test/image build), Azure Container Apps deployment docs | 🔜 |

## Design notes

- **Choreography over orchestration**: services react to each other's events independently rather than through a central saga coordinator. Simpler to build, harder to trace at scale — a deliberate tradeoff for this project's size.
- **RabbitMQ on Azure**: not a native PaaS offering. Options for Phase 6: run RabbitMQ as a sidecar container in the same Azure Container Apps environment, or use CloudAMQP's free tier.
- **Auth**: a minimal JWT-issuing `Identity.Api` rather than full Duende IdentityServer — enough to demonstrate the pattern without licensing/complexity overhead disproportionate to a portfolio project.
