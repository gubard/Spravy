using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Spravy.Db.Sqlite.Models;
using Spravy.Domain.Interfaces;

namespace Spravy.Db.Sqlite.Extensions;

public static class DbContextOptionsBuilderExtension
{
    public static DbContextOptionsBuilder UseSqliteFile<TAssemblyMark>(
        this DbContextOptionsBuilder optionsBuilder,
        SqliteFileOptions options
    ) where TAssemblyMark : IAssemblyMark
    {
        optionsBuilder.UseSqlite(
            options.ToSqliteConnectionString(),
            b => b.MigrationsAssembly(TAssemblyMark.AssemblyFullName)
        );

        return optionsBuilder;
    }

    public static DbContextOptionsBuilder UseSqliteFile<TAssemblyMark>(
        this DbContextOptionsBuilder optionsBuilder,
        IServiceProvider serviceProvider
    )
        where TAssemblyMark : IAssemblyMark
    {
        optionsBuilder.UseSqliteFile<TAssemblyMark>(serviceProvider.GetRequiredService<SqliteFileOptions>());

        return optionsBuilder;
    }
}