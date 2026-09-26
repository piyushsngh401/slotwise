using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Slotwise.SharedKernel.Modules;

public static class ModuleCompositionExtensions
{
    public const string ApiBasePath = "/api/v1";

    /// <summary>
    /// Registers each module's services and records the module set for endpoint mapping.
    /// </summary>
    public static IHostApplicationBuilder AddModules(
        this IHostApplicationBuilder builder,
        IEnumerable<IModule> modules)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(modules);

        var moduleList = modules.ToList();
        EnsureUniqueRoutePrefixes(moduleList);

        foreach (var module in moduleList)
        {
            module.RegisterServices(builder);
        }

        builder.Services.AddSingleton<IReadOnlyList<IModule>>(moduleList);
        return builder;
    }

    /// <summary>
    /// Maps every registered module under <c>/api/v1/{prefix}</c>, tagged with the module name.
    /// </summary>
    public static WebApplication MapModules(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        var modules = app.Services.GetRequiredService<IReadOnlyList<IModule>>();
        var api = app.MapGroup(ApiBasePath);

        foreach (var module in modules)
        {
            var group = api.MapGroup($"/{module.RoutePrefix}").WithTags(module.Name);
            module.MapEndpoints(group);
        }

        return app;
    }

    private static void EnsureUniqueRoutePrefixes(List<IModule> modules)
    {
        var duplicate = modules
            .GroupBy(m => m.RoutePrefix, StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault(g => g.Count() > 1);

        if (duplicate is not null)
        {
            throw new InvalidOperationException(
                $"More than one module uses the route prefix '{duplicate.Key}'.");
        }
    }
}
