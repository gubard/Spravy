using System.Reflection;

namespace Spravy.Authentication.Db.Sqlite.Migrator;

public readonly struct SpravyAuthenticationDbSqliteMigratorMark
{
    public static readonly AssemblyName AssemblyName = typeof(SpravyAuthenticationDbSqliteMigratorMark).Assembly.GetName();
    public static readonly string AssemblyFullName = typeof(SpravyAuthenticationDbSqliteMigratorMark).Assembly.GetName().FullName;
    public static readonly Assembly Assembly = typeof(SpravyAuthenticationDbSqliteMigratorMark).Assembly;
}