using System.Reflection;

namespace Spravy.EventBus.Service;

public struct SpravyEventBusServiceMark : IAssemblyMark
{
    public static AssemblyName AssemblyName { get; } = typeof(SpravyEventBusServiceMark).Assembly.GetName();

    public static string AssemblyFullName { get; } = typeof(SpravyEventBusServiceMark).Assembly.GetName().FullName;

    public static Assembly Assembly { get; } = typeof(SpravyEventBusServiceMark).Assembly;

    public static Stream? GetResourceStream(string resourceName)
    {
        return Assembly.GetManifestResourceStream($"{AssemblyName.Name}.{resourceName}");
    }
}