using System.Reflection;
using Spravy.Domain.Interfaces;

namespace Spravy.Picture.Service;

public readonly struct SpravyPictureMark : IAssemblyMark
{
    public static AssemblyName AssemblyName { get; } = typeof(SpravyPictureMark).Assembly.GetName();
    public static string AssemblyFullName { get; } = typeof(SpravyPictureMark).Assembly.GetName().FullName;
    public static Assembly Assembly { get; } = typeof(SpravyPictureMark).Assembly;

    public static Stream? GetResourceStream(string resourceName)
    {
        return Assembly.GetManifestResourceStream($"{AssemblyName.Name}.{resourceName}");
    }
}