using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Slotwise.SharedKernel.Modules;

namespace Slotwise.Api.IntegrationTests;

/// <summary>
/// Verifies the module composition contract in isolation, using a throwaway module,
/// so the contract is covered before any real business module exists.
/// </summary>
public sealed class ModuleCompositionTests
{
    [Fact]
    public async Task Module_endpoints_are_mapped_under_api_v1_and_its_prefix()
    {
        await using var app = await StartAppAsync(new PingModule("ping"));
        var client = app.GetTestClient();

        var response = await client.GetAsync("/api/v1/ping", TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken)).ShouldBe("pong");
    }

    [Fact]
    public async Task Module_services_are_registered()
    {
        await using var app = await StartAppAsync(new PingModule("ping"));

        app.Services.GetService<PingService>().ShouldNotBeNull();
    }

    [Fact]
    public void Duplicate_route_prefixes_are_rejected_at_startup()
    {
        var builder = WebApplication.CreateBuilder();

        var act = () => builder.AddModules([new PingModule("ping"), new PingModule("PING")]);

        act.ShouldThrow<InvalidOperationException>().Message.ShouldContain("ping");
    }

    private static async Task<WebApplication> StartAppAsync(params IModule[] modules)
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        builder.AddModules(modules);

        var app = builder.Build();
        app.MapModules();
        await app.StartAsync(TestContext.Current.CancellationToken);
        return app;
    }

    private sealed class PingService
    {
        public string Reply => "pong";
    }

    private sealed class PingModule(string routePrefix) : IModule
    {
        public string Name => "Ping";

        public string RoutePrefix => routePrefix;

        public void RegisterServices(IHostApplicationBuilder builder) =>
            builder.Services.AddSingleton<PingService>();

        public void MapEndpoints(IEndpointRouteBuilder endpoints) =>
            endpoints.MapGet("/", (PingService ping) => ping.Reply);
    }
}
