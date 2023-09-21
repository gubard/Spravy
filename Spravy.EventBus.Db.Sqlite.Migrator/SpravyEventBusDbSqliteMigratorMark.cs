using System.Reflection;

namespace Spravy.EventBus.Db.Sqlite.Migrator;

public readonly struct SpravyEventBusDbSqliteMigratorMark
{
    public static readonly AssemblyName AssemblyName = typeof(SpravyEventBusDbSqliteMigratorMark).Assembly.GetName();
    public static readonly string AssemblyFullName = typeof(SpravyEventBusDbSqliteMigratorMark).Assembly.GetName().FullName;
    public static readonly Assembly Assembly = typeof(SpravyEventBusDbSqliteMigratorMark).Assembly;
}