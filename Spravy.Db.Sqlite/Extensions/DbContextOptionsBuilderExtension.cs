using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Spravy.Db.Sqlite.Models;

namespace Spravy.Db.Sqlite.Extensions;

public static class DbContextOptionsBuilderExtension
{
    public static DbContextOptionsBuilder UseSqliteFile(
        this DbContextOptionsBuilder optionsBuilder,
        SqliteFileOptions options
    )
    {
        optionsBuilder.UseSqlite(options.ToSqliteConnectionString());

        return optionsBuilder;
    }

    public static DbContextOptionsBuilder UseSqliteFile(
        this DbContextOptionsBuilder optionsBuilder,
        IServiceProvider serviceProvider
    )
    {
        optionsBuilder.UseSqliteFile(serviceProvider.GetRequiredService<SqliteFileOptions>());

        return optionsBuilder;
    }
}