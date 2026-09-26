using System.Reflection;
using NetArchTest.Rules;
using Slotwise.SharedKernel.Modules;

namespace Slotwise.ArchitectureTests;

/// <summary>
/// Executable versions of the rules in docs/adr/0002-modular-monolith.md.
/// If one of these fails, the change is breaking a module boundary, not just a test.
/// </summary>
public sealed class DependencyRulesTests
{
    private const string ModuleAssemblyPrefix = "Slotwise.Modules.";

    private static readonly Assembly _sharedKernel = typeof(IModule).Assembly;
    private static readonly Assembly _apiHost = typeof(Program).Assembly;

    [Fact]
    public void SharedKernel_does_not_depend_on_the_host_or_any_module()
    {
        var result = Types.InAssembly(_sharedKernel)
            .ShouldNot()
            .HaveDependencyOnAny("Slotwise.Api", "Slotwise.ServiceDefaults", "Slotwise.Modules")
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(DescribeViolations(result.FailingTypeNames));
    }

    [Fact]
    public void Modules_do_not_reference_each_other()
    {
        // Modules may depend on SharedKernel and on other modules' *.Contracts assemblies only.
        // Vacuously true until Phase 2 adds the first module; it guards every module after that.
        foreach (var module in LoadModuleAssemblies())
        {
            var forbidden = module.GetReferencedAssemblies()
                .Select(reference => reference.Name!)
                .Where(name => name.StartsWith(ModuleAssemblyPrefix, StringComparison.Ordinal))
                .Where(name => !name.EndsWith(".Contracts", StringComparison.Ordinal))
                .ToList();

            forbidden.ShouldBeEmpty(
                $"{module.GetName().Name} must talk to other modules through their Contracts assemblies.");
        }
    }

    private static IEnumerable<Assembly> LoadModuleAssemblies() =>
        _apiHost.GetReferencedAssemblies()
            .Where(reference => reference.Name!.StartsWith(ModuleAssemblyPrefix, StringComparison.Ordinal))
            .Select(Assembly.Load);

    private static string DescribeViolations(IEnumerable<string>? failingTypeNames) =>
        failingTypeNames is null
            ? string.Empty
            : "Violations: " + string.Join(", ", failingTypeNames);
}
