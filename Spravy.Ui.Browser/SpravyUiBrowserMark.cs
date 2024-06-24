using System.Reflection;
using Spravy.Domain.Interfaces;

namespace Spravy.Ui.Browser;

public struct SpravyUiBrowserMark : IAssemblyMark
{
    public static AssemblyName AssemblyName { get; } = typeof(SpravyUiBrowserMark).Assembly.GetName();
    public static string AssemblyFullName { get; } = typeof(SpravyUiBrowserMark).Assembly.GetName().FullName;
    public static Assembly Assembly { get; } = typeof(SpravyUiBrowserMark).Assembly;

    public static Stream? GetResourceStream(string resourceName)
    {
        return Assembly.GetManifestResourceStream($"{AssemblyName.Name}.{resourceName}");
    }
}