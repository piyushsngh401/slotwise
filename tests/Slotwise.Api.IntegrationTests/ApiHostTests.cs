using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Slotwise.Api.IntegrationTests;

/// <summary>Boots the real API host in memory and checks its platform endpoints.</summary>
public sealed class ApiHostTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Theory]
    [InlineData("/health")]
    [InlineData("/alive")]
    public async Task Health_endpoints_report_healthy(string path)
    {
        var response = await _client.GetAsync(path, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))
            .ShouldBe("Healthy");
    }

    [Fact]
    public async Task Root_returns_service_info()
    {
        var info = await _client.GetFromJsonAsync<ServiceInfo>("/", TestContext.Current.CancellationToken);

        info.ShouldNotBeNull();
        info.Service.ShouldBe("Slotwise API");
        info.Version.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task OpenApi_document_is_served_in_development()
    {
        var response = await _client.GetAsync("/openapi/v1.json", TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Unknown_route_returns_problem_details()
    {
        var response = await _client.GetAsync("/api/v1/does-not-exist", TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/problem+json");
    }
}
