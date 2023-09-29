using System.Reflection;
using Spravy.Domain.Interfaces;

namespace Spravy.EventBus.Db.Sqlite.Migrator;

public readonly struct SpravyEventBusDbSqliteMigratorMark : IAssemblyMark
{
    public static AssemblyName AssemblyName { get; } = typeof(SpravyEventBusDbSqliteMigratorMark).Assembly.GetName();

    public static string AssemblyFullName { get; } =
        typeof(SpravyEventBusDbSqliteMigratorMark).Assembly.GetName().FullName;

    public static Assembly Assembly { get; } = typeof(SpravyEventBusDbSqliteMigratorMark).Assembly;
}