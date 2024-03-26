using System.Reflection;
using Spravy.Domain.Interfaces;

namespace Spravy.PasswordGenerator.Service;

public readonly struct SpravyPasswordGeneratorMark : IAssemblyMark
{
    public static AssemblyName AssemblyName { get; } =
        typeof(SpravyPasswordGeneratorMark).Assembly.GetName();

    public static string AssemblyFullName { get; } =
        typeof(SpravyPasswordGeneratorMark).Assembly.GetName().FullName;

    public static Assembly Assembly { get; } = typeof(SpravyPasswordGeneratorMark).Assembly;

    public static Stream? GetResourceStream(string resourceName)
    {
        return Assembly.GetManifestResourceStream($"{AssemblyName.Name}.{resourceName}");
    }
}