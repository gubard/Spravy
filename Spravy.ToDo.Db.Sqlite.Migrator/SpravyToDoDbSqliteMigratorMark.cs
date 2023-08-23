using System.Reflection;

namespace Spravy.ToDo.Db.Sqlite.Migrator;

public readonly struct SpravyToDoDbSqliteMigratorMark
{
    public static readonly AssemblyName AssemblyName = typeof(SpravyToDoDbSqliteMigratorMark).Assembly.GetName();
    public static readonly string AssemblyFullName = typeof(SpravyToDoDbSqliteMigratorMark).Assembly.GetName().FullName;
    public static readonly Assembly Assembly = typeof(SpravyToDoDbSqliteMigratorMark).Assembly;
}