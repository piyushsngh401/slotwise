# 0001. Record architecture decisions

- **Status:** Accepted
- **Date:** 2026-09-26

## Context

Slotwise is built in phases over several weeks. Decisions made in Phase 1 shape Phase 8, and
code shows *what* was decided but rarely *why*, or which alternatives were rejected.

## Decision

Record every architecturally significant decision as a short Markdown ADR in `docs/adr`,
numbered sequentially, using the [template](template.md). An accepted ADR is not rewritten;
a changed decision gets a new ADR that supersedes the old one.

A decision is significant when it is expensive to reverse, affects more than one module,
or would surprise a new contributor.

## Consequences

- Reviewers can challenge the reasoning, not just the code.
- The history of trade-offs is preserved alongside the code that implements them.
- Writing an ADR adds a small amount of work to each significant change.
