using System.Reflection;
using Spravy.Domain.Interfaces;

namespace Spravy.Schedule.Db.Sqlite.Migrator;

public readonly struct SpravyScheduleDbSqliteMigratorMark : IAssemblyMark
{
    public static AssemblyName AssemblyName { get; } = typeof(SpravyScheduleDbSqliteMigratorMark).Assembly.GetName();

    public static string AssemblyFullName { get; } =
        typeof(SpravyScheduleDbSqliteMigratorMark).Assembly.GetName().FullName;

    public static Assembly Assembly { get; } = typeof(SpravyScheduleDbSqliteMigratorMark).Assembly;

    public static Stream? GetResourceStream(string resourceName)
    {
        return Assembly.GetManifestResourceStream($"{AssemblyName.Name}.{resourceName}");
    }
}