namespace Spravy.Schedule.Service;

public struct SpravyScheduleServiceMark : IAssemblyMark
{
    public static AssemblyName AssemblyName { get; } = typeof(SpravyScheduleServiceMark).Assembly.GetName();

    public static string AssemblyFullName { get; } = typeof(SpravyScheduleServiceMark).Assembly.GetName().FullName;

    public static Assembly Assembly { get; } = typeof(SpravyScheduleServiceMark).Assembly;

    public static Stream? GetResourceStream(string resourceName)
    {
        return Assembly.GetManifestResourceStream($"{AssemblyName.Name}.{resourceName}");
    }
}