using System.Reflection;
using Spravy.Domain.Interfaces;

namespace Spravy.Authentication.Service;

public struct SpravyAuthenticationServiceMark : IAssemblyMark
{
    public static AssemblyName AssemblyName { get; } =
        typeof(SpravyAuthenticationServiceMark).Assembly.GetName();

    public static string AssemblyFullName { get; } =
        typeof(SpravyAuthenticationServiceMark).Assembly.GetName().FullName;

    public static Assembly Assembly { get; } = typeof(SpravyAuthenticationServiceMark).Assembly;

    public static Stream? GetResourceStream(string resourceName)
    {
        return Assembly.GetManifestResourceStream($"{AssemblyName.Name}.{resourceName}");
    }
}