using System.Reflection;
using Spravy.Domain.Interfaces;

namespace Spravy.Authentication.Db.Sqlite.Migrator;

public readonly struct SpravyAuthenticationDbSqliteMigratorMark : IAssemblyMark
{
    public static AssemblyName AssemblyName { get; } =
        typeof(SpravyAuthenticationDbSqliteMigratorMark).Assembly.GetName();

    public static string AssemblyFullName { get; } =
        typeof(SpravyAuthenticationDbSqliteMigratorMark).Assembly.GetName().FullName;

    public static Assembly Assembly { get; } = typeof(SpravyAuthenticationDbSqliteMigratorMark).Assembly;

    public static Stream? GetResourceStream(string resourceName)
    {
        return Assembly.GetManifestResourceStream($"{AssemblyName.Name}.{resourceName}");
    }
}