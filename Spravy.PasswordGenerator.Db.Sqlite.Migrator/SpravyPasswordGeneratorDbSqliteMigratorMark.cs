using System.Reflection;
using Spravy.Domain.Interfaces;

namespace Spravy.PasswordGenerator.Db.Sqlite.Migrator;

public readonly struct SpravyPasswordGeneratorDbSqliteMigratorMark : IAssemblyMark
{
    public static AssemblyName AssemblyName { get; } =
        typeof(SpravyPasswordGeneratorDbSqliteMigratorMark).Assembly.GetName();

    public static string AssemblyFullName { get; } =
        typeof(SpravyPasswordGeneratorDbSqliteMigratorMark).Assembly.GetName().FullName;

    public static Assembly Assembly { get; } = typeof(SpravyPasswordGeneratorDbSqliteMigratorMark).Assembly;

    public static Stream? GetResourceStream(string resourceName)
    {
        return Assembly.GetManifestResourceStream($"{AssemblyName.Name}.{resourceName}");
    }
}