using System.Reflection;

namespace Spravy.Ui.Android;

public struct SpravyUiAndroidMark : IAssemblyMark
{
    public static AssemblyName AssemblyName { get; } = typeof(SpravyUiAndroidMark).Assembly.GetName();
    public static string AssemblyFullName { get; } = typeof(SpravyUiAndroidMark).Assembly.GetName().FullName;
    public static Assembly Assembly { get; } = typeof(SpravyUiAndroidMark).Assembly;

    public static Stream? GetResourceStream(string resourceName)
    {
        return Assembly.GetManifestResourceStream($"{AssemblyName.Name}.{resourceName}");
    }
}