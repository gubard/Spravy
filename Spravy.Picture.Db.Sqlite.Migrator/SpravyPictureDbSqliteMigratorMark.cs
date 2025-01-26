using System.Reflection;
using Spravy.Domain.Interfaces;

namespace Spravy.Picture.Db.Sqlite.Migrator;

public readonly struct SpravyPictureDbSqliteMigratorMark : IAssemblyMark
{
    public static AssemblyName AssemblyName { get; } = typeof(SpravyPictureDbSqliteMigratorMark).Assembly.GetName();
    public static string AssemblyFullName { get; } = typeof(SpravyPictureDbSqliteMigratorMark).Assembly.GetName().FullName;
    public static Assembly Assembly { get; } = typeof(SpravyPictureDbSqliteMigratorMark).Assembly;

    public static Stream? GetResourceStream(string resourceName)
    {
        return Assembly.GetManifestResourceStream($"{AssemblyName.Name}.{resourceName}");
    }
}