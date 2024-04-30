using _build.Interfaces;

namespace _build.Extensions;

public static class CsprojFileExtension
{
    public static string GetProjectName(this ICsprojFile csprojFile) =>
        csprojFile.CsprojFile.GetFileNameWithoutExtension();

    public static string GetServiceName(this ICsprojFile csprojFile) => csprojFile.GetProjectName().ToLower();
}