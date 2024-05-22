namespace Spravy.Db.Sqlite.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddSpravySqliteFileDbContext<TContext, TAssemblyMark>(
        this IServiceCollection serviceCollection
    ) where TContext : DbContext where TAssemblyMark : IAssemblyMark
    {
        serviceCollection.AddDbContext<TContext>((sp, x) => x.UseSqliteFile<TAssemblyMark>(sp));

        return serviceCollection;
    }
}