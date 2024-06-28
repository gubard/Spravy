using System.Reflection;
using Spravy.Domain.Interfaces;

namespace Spravy.Router.Service;

public struct SpravyRouterServiceMark : IAssemblyMark
{
    public static AssemblyName AssemblyName { get; } =
        typeof(SpravyRouterServiceMark).Assembly.GetName();

    public static string AssemblyFullName { get; } =
        typeof(SpravyRouterServiceMark).Assembly.GetName().FullName;

    public static Assembly Assembly { get; } = typeof(SpravyRouterServiceMark).Assembly;

    public static Stream? GetResourceStream(string resourceName)
    {
        return Assembly.GetManifestResourceStream($"{AssemblyName.Name}.{resourceName}");
    }
}
