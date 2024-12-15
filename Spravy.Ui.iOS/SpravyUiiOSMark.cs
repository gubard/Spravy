using System.Reflection;
using Spravy.Domain.Interfaces;

namespace Spravy.Ui.iOS;

public struct SpravyUiiOSMark : IAssemblyMark
{
    public static AssemblyName AssemblyName { get; } = typeof(SpravyUiiOSMark).Assembly.GetName();
    public static string AssemblyFullName { get; } = typeof(SpravyUiiOSMark).Assembly.GetName().FullName;
    public static Assembly Assembly { get; } = typeof(SpravyUiiOSMark).Assembly;

    public static Stream? GetResourceStream(string resourceName)
    {
        return Assembly.GetManifestResourceStream($"{AssemblyName.Name}.{resourceName}");
    }
}