# Architecture Decision Records

Significant decisions are recorded here so the reasoning survives longer than anyone's memory of it.
Each record is short, immutable once accepted, and superseded (not edited) when the decision changes.

| #    | Decision                                                                 | Status   |
| ---- | ------------------------------------------------------------------------ | -------- |
| 0001 | [Record architecture decisions](0001-record-architecture-decisions.md)   | Accepted |
| 0002 | [Start as a modular monolith](0002-modular-monolith.md)                  | Accepted |
| 0003 | [Use .NET Aspire for local orchestration](0003-aspire-for-local-orchestration.md) | Accepted |
| 0004 | [Build quality gates](0004-build-quality-gates.md)                       | Accepted |
| 0005 | [Testing strategy](0005-testing-strategy.md)                             | Accepted |

Planned for later phases: tenant isolation strategy (Phase 2), CQRS scope (Phase 3),
identity provider (Phase 4), messaging and the outbox pattern (Phase 5), LLM provider abstraction (Phase 7).

New records start from [the template](template.md).
