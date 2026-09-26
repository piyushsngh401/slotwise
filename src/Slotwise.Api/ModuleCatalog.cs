using Slotwise.SharedKernel.Modules;

namespace Slotwise.Api;

/// <summary>
/// The single place where business modules are composed into the host.
/// Composition is explicit rather than assembly-scanned so the dependency graph is visible
/// in code review and nothing is registered by accident. See docs/adr/0002-modular-monolith.md.
/// </summary>
internal static class ModuleCatalog
{
    public static IReadOnlyList<IModule> All { get; } =
    [
        // Phase 2: Tenants, Catalog, Scheduling
        // Phase 3: Bookings
    ];
}
