using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spravy.Db.Sqlite.Helpers;
using Spravy.Db.Sqlite.Models;

namespace Spravy.Db.Sqlite.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddSpravyDbContext<TContext>(this IServiceCollection serviceCollection)
        where TContext : DbContext
    {
        serviceCollection.AddDbContext<TContext>(
            (sp, x) => x.UseSqlite($"DataSource={sp.GetRequiredService<SqliteFileOptions>().DataBaseFile}")
        );

        return serviceCollection;
    }
}