namespace Spravy.Db.Sqlite.Extensions;

public static class DbContextOptionsBuilderExtension
{
    public static DbContextOptionsBuilder UseSqliteFile<TAssemblyMark>(
        this DbContextOptionsBuilder optionsBuilder,
        SqliteFileOptions options,
        IDbFileSystem dbFileSystem
    ) where TAssemblyMark : IAssemblyMark
    {
        optionsBuilder.UseSqlite(
            dbFileSystem.GetDbFile(options.DataBaseFile.ThrowIfNullOrWhiteSpace()).ToSqliteConnectionString(),
            b => b.MigrationsAssembly(TAssemblyMark.AssemblyFullName)
        );

        return optionsBuilder;
    }

    public static DbContextOptionsBuilder UseSqliteFile<TAssemblyMark>(
        this DbContextOptionsBuilder optionsBuilder,
        IServiceProvider serviceProvider
    ) where TAssemblyMark : IAssemblyMark
    {
        optionsBuilder.UseSqliteFile<TAssemblyMark>(serviceProvider.GetRequiredService<SqliteFileOptions>(), serviceProvider.GetRequiredService<IDbFileSystem>());

        return optionsBuilder;
    }
}