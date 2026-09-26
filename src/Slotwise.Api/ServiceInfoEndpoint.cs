using System.Reflection;

namespace Slotwise.Api;

/// <summary>Basic service metadata at the root URL, handy for smoke tests and deployments.</summary>
internal static class ServiceInfoEndpoint
{
    public static IEndpointRouteBuilder MapServiceInfo(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/", (IHostEnvironment environment) => TypedResults.Ok(new ServiceInfo(
                Service: "Slotwise API",
                Version: GetVersion(),
                Environment: environment.EnvironmentName)))
            .WithName("GetServiceInfo")
            .WithTags("Service")
            .ExcludeFromDescription();

        return endpoints;
    }

    private static string GetVersion() =>
        typeof(ServiceInfoEndpoint).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion ?? "unknown";
}

internal sealed record ServiceInfo(string Service, string Version, string Environment);
