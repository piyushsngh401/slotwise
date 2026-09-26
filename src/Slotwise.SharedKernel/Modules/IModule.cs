using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;

namespace Slotwise.SharedKernel.Modules;

/// <summary>
/// A vertical slice of the business (Tenants, Scheduling, Bookings, ...).
/// Each module owns its services, data and endpoints, and is composed into the API host
/// explicitly. See docs/adr/0002-modular-monolith.md.
/// </summary>
public interface IModule
{
    /// <summary>Human-readable module name, used as the OpenAPI tag.</summary>
    string Name { get; }

    /// <summary>URL segment under <c>/api/v1</c>, for example <c>bookings</c>.</summary>
    string RoutePrefix { get; }

    /// <summary>Registers the module's services. Called once at startup.</summary>
    void RegisterServices(IHostApplicationBuilder builder);

    /// <summary>Maps the module's endpoints onto a route group already scoped to its prefix.</summary>
    void MapEndpoints(IEndpointRouteBuilder endpoints);
}
