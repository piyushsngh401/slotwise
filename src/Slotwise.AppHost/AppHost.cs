// Local development orchestration. `dotnet run --project src/Slotwise.AppHost` starts
// every Slotwise resource plus the Aspire dashboard (logs, traces, metrics).
// Infrastructure is added phase by phase: PostgreSQL (Phase 2), Keycloak (Phase 4),
// RabbitMQ (Phase 5), Redis (Phase 6). See docs/adr/0003-aspire-for-local-orchestration.md.

var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Slotwise_Api>("api")
    .WithHttpHealthCheck("/health")
    .WithExternalHttpEndpoints();

builder.Build().Run();
