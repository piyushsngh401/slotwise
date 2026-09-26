# 0002. Start as a modular monolith

- **Status:** Accepted
- **Date:** 2026-09-26

## Context

Slotwise has several distinct business capabilities: tenants, service catalog, staff
scheduling, bookings, payments and notifications. These could be separate microservices
from day one. However:

- The domain boundaries are still being discovered. Drawing a network boundary in the wrong
  place is far more expensive to fix than drawing a module boundary in the wrong place.
- Microservices add operational cost (deployments, distributed tracing, network failures,
  data consistency across services) before there is any scaling need that justifies it.
- The team is small. Independent deployability matters less than delivery speed.

## Options considered

1. **Microservices from the start.** Strong isolation and independent scaling, but high
   operational overhead and costly boundary mistakes.
2. **Traditional layered monolith.** Simplest to start, but boundaries erode over time and
   extracting a service later means untangling a shared data model.
3. **Modular monolith.** One deployable unit, with modules that own their code, data and
   endpoints and communicate only through explicit contracts.

## Decision

Build a **modular monolith**, designed so any module can be extracted into its own service later.

Rules every module follows:

1. A module is a vertical slice that implements `IModule` and is composed explicitly in
   `ModuleCatalog`. Composition is explicit rather than assembly-scanned, so the dependency
   graph is visible in code review.
2. A module owns its data. Each module gets its own database schema; no module queries
   another module's tables.
3. Modules talk to each other only through a module's `*.Contracts` assembly
   (queries or integration events), never through internal types.
4. `Slotwise.SharedKernel` stays small and never depends on the host or on any module.

Rules 3 and 4 are enforced by `tests/Slotwise.ArchitectureTests`, so a boundary violation
fails the build instead of relying on code review.

## Consequences

- One process, one deployment and in-process calls keep Phases 2–4 fast to build.
- Integration events between modules (Phase 5) go through the same outbox and broker a
  microservice would use, so extracting a module later changes deployment, not design.
- Modules can't be scaled independently. This is acceptable until load testing (Phase 8)
  shows a specific module needs it.
- Discipline is required, which is why the rules are automated rather than documented only.
