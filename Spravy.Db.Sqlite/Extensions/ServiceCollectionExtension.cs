using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Spravy.Db.Sqlite.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddSpravySqliteFileDbContext<TContext>(this IServiceCollection serviceCollection)
        where TContext : DbContext
    {
        serviceCollection.AddDbContext<TContext>((sp, x) => x.UseSqliteFile(sp));

        return serviceCollection;
    }
}