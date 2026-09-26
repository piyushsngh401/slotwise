# 0005. Testing strategy

- **Status:** Accepted
- **Date:** 2026-09-26

## Context

The tests need to give confidence to refactor module internals freely, catch boundary
violations, and exercise real infrastructure behaviour (transactions, concurrency,
messaging) that mocks tend to hide.

## Decision

Test in layers, from fastest to most realistic:

| Layer        | Tooling                                   | Covers                                                        | From    |
| ------------ | ----------------------------------------- | ------------------------------------------------------------- | ------- |
| Architecture | NetArchTest, reflection                   | Module boundary rules from ADR-0002                           | Phase 1 |
| Unit         | xUnit v3, Shouldly                        | Domain logic: availability rules, booking invariants          | Phase 2 |
| Integration  | `WebApplicationFactory`, Testcontainers   | HTTP API against real PostgreSQL, RabbitMQ and Redis          | Phase 1 (host), Phase 2 (database) |
| Load         | k6                                        | Booking throughput and contention on popular slots            | Phase 8 |

Library choices:

- **xUnit v3:** current major version with first-class cancellation and in-process runner support.
- **Shouldly** for assertions: readable failure messages, and MIT licensed. FluentAssertions
  moved to a commercial licence for version 8, which is a poor fit for an open-source project.
- **Testcontainers** instead of in-memory database providers, because the EF Core in-memory
  provider doesn't behave like PostgreSQL for transactions, constraints or concurrency.

## Consequences

- Integration tests need Docker from Phase 2; GitHub-hosted Ubuntu runners provide it.
- Tests favour behaviour through public APIs over mocking internals, so refactoring
  a module rarely breaks its tests.
- The integration suite is slower than unit tests. It stays in the default CI run
  until its duration becomes a problem.
