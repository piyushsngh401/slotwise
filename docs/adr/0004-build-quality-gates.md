# 0004. Build quality gates

- **Status:** Accepted
- **Date:** 2026-09-26

## Context

Code quality tends to drift when rules live only in reviewers' heads. The cheapest place
to catch problems is the build, before a human reviews anything.

## Decision

- **Pinned SDK:** `global.json` pins the .NET 10 SDK feature band so local and CI builds agree.
- **Shared build settings:** `Directory.Build.props` applies the target framework, nullable
  reference types, the latest analyzers and **warnings as errors** to every project.
- **Central Package Management:** `Directory.Packages.props` holds every NuGet version once,
  with transitive pinning, so projects cannot drift onto different versions.
- **Code style in the build:** `.editorconfig` rules run at build time
  (`EnforceCodeStyleInBuild`), and CI runs `dotnet format --verify-no-changes`.
- **CI on every pull request:** restore, format check, build and test must pass before merge.
- **Dependabot** proposes grouped NuGet and GitHub Actions updates weekly.

## Consequences

- Warnings can't accumulate; each one is fixed or explicitly suppressed with a reason.
- Upgrading a package is a one-line change in one file.
- Occasionally a new analyzer rule in an SDK update will fail the build and need a
  deliberate decision to fix or suppress it.
