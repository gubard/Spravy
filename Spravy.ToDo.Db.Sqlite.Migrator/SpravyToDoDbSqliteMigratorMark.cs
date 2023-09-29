using System.Reflection;
using Spravy.Domain.Interfaces;

namespace Spravy.ToDo.Db.Sqlite.Migrator;

public readonly struct SpravyToDoDbSqliteMigratorMark : IAssemblyMark
{
    public static AssemblyName AssemblyName { get; } = typeof(SpravyToDoDbSqliteMigratorMark).Assembly.GetName();
    public static string AssemblyFullName { get; } = typeof(SpravyToDoDbSqliteMigratorMark).Assembly.GetName().FullName;
    public static Assembly Assembly { get; } = typeof(SpravyToDoDbSqliteMigratorMark).Assembly;
}