using _build.Interfaces;

namespace _build.Extensions;

public static class CsprojFileExtension
{
    public static string GetProjectName(this ICsprojFile csprojFile)
    {
        return csprojFile.CsprojFile.GetFileNameWithoutExtension();
    }
    
    public static string GetServiceName(this ICsprojFile csprojFile)
    {
        return csprojFile.GetProjectName().ToLower();
    }
}