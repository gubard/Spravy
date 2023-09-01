using System.Reflection;

namespace Spravy.Schedule.Db.Sqlite.Migrator;

public readonly struct SpravyScheduleDbSqliteMigratorMark
{
    public static readonly AssemblyName AssemblyName = typeof(SpravyScheduleDbSqliteMigratorMark).Assembly.GetName();
    public static readonly string AssemblyFullName = typeof(SpravyScheduleDbSqliteMigratorMark).Assembly.GetName().FullName;
    public static readonly Assembly Assembly = typeof(SpravyScheduleDbSqliteMigratorMark).Assembly;
}