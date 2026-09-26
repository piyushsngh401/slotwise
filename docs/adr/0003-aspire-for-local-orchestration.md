# 0003. Use .NET Aspire for local orchestration

- **Status:** Accepted
- **Date:** 2026-09-26

## Context

Slotwise will depend on PostgreSQL, Redis, RabbitMQ and Keycloak. Every developer needs to
run all of these locally with correct connection strings, and needs to see logs, traces and
metrics across them while debugging.

## Options considered

1. **docker-compose plus manual configuration.** Familiar and tool-agnostic, but connection
   strings are duplicated between compose files and app settings, and there is no built-in
   telemetry view.
2. **.NET Aspire AppHost.** The application model is written in C#; Aspire starts containers,
   injects connection strings and service discovery, and ships a dashboard for OpenTelemetry data.
3. **Local Kubernetes (kind, minikube).** Closest to production, but a slow inner loop and
   heavy for day-to-day development.

## Decision

Use **.NET Aspire** (`src/Slotwise.AppHost`) for local development and
`Slotwise.ServiceDefaults` for cross-cutting concerns (OpenTelemetry, health checks,
resilience, service discovery).

Aspire is a **development-time** tool here. Production deployment uses Kubernetes manifests
or Helm (Phase 8), and the services have no runtime dependency on Aspire: they read standard
configuration and export standard OTLP telemetry.

## Consequences

- `dotnet run --project src/Slotwise.AppHost` starts the whole system with telemetry.
- Infrastructure is added to the AppHost in the phase that needs it, not all at once.
- Contributors need Docker for later phases and must learn the Aspire application model.
- Keeping production deployment separate avoids lock-in, but means maintaining the
  Kubernetes configuration alongside the AppHost.
