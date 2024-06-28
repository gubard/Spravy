using System.Reflection;
using Spravy.Domain.Interfaces;

namespace Spravy.ToDo.Service;

public struct SpravyToDoServiceMark : IAssemblyMark
{
    public static AssemblyName AssemblyName { get; } =
        typeof(SpravyToDoServiceMark).Assembly.GetName();

    public static string AssemblyFullName { get; } =
        typeof(SpravyToDoServiceMark).Assembly.GetName().FullName;

    public static Assembly Assembly { get; } = typeof(SpravyToDoServiceMark).Assembly;

    public static Stream? GetResourceStream(string resourceName)
    {
        return Assembly.GetManifestResourceStream($"{AssemblyName.Name}.{resourceName}");
    }
}
