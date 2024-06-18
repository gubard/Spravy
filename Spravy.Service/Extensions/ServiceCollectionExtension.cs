using Spravy.Core.Extensions;

namespace Spravy.Service.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddSpravySqliteFolderContext<TDbContext, TAssemblyMark, TDbContextSetup>(
        this IServiceCollection serviceCollection
    )
        where TDbContext : SpravyDbContext, IDbContextCreator<TDbContext>
        where TAssemblyMark : IAssemblyMark
        where TDbContextSetup : IDbContextSetup
    {
        serviceCollection.AddTransient<IFactory<FileInfo>, SqliteDbFileFactory>();
        serviceCollection.AddTransient<IFactory<TDbContext>, SqliteDbContextFactory<TDbContext>>();
        serviceCollection.AddTransient<ICacheValidator<string, TDbContext>, DbContextCacheValidator<TDbContext>>();

        serviceCollection.AddTransient(sp =>
            new DbContextFactory<TDbContext, TAssemblyMark>(sp.GetRequiredService<TDbContextSetup>()));

        serviceCollection.AddTransient<IFactory<string, TDbContext>>(sp =>
            new CacheFactory<string, TDbContext>(sp.GetRequiredService<DbContextFactory<TDbContext, TAssemblyMark>>(),
                sp.GetRequiredService<ICacheValidator<string, TDbContext>>()));

        return serviceCollection;
    }

    public static IServiceCollection AddSpravySqliteFolderContext<TDbContext, TAssemblyMark>(
        this IServiceCollection serviceCollection
    ) where TDbContext : SpravyDbContext, IDbContextCreator<TDbContext> where TAssemblyMark : IAssemblyMark
    {
        serviceCollection.AddTransient<IFactory<FileInfo>, SqliteDbFileFactory>();
        serviceCollection.AddTransient<IFactory<TDbContext>, SqliteDbContextFactory<TDbContext>>();
        serviceCollection.AddTransient<DbContextFactory<TDbContext, TAssemblyMark>>();
        serviceCollection.AddTransient<ICacheValidator<string, TDbContext>, DbContextCacheValidator<TDbContext>>();

        serviceCollection.AddTransient<IFactory<string, TDbContext>>(sp =>
            new CacheFactory<string, TDbContext>(sp.GetRequiredService<DbContextFactory<TDbContext, TAssemblyMark>>(),
                sp.GetRequiredService<ICacheValidator<string, TDbContext>>()));

        return serviceCollection;
    }

    public static IServiceCollection AddSpravy<TAssemblyMark>(
        this IServiceCollection serviceCollection,
        IConfiguration configuration
    ) where TAssemblyMark : IAssemblyMark
    {
        serviceCollection.AddGrpc();
        serviceCollection.AddAuthorization();
        serviceCollection.AddHttpContextAccessor();
        serviceCollection.AddCors(o => o.AddAllowAllPolicy());
        serviceCollection.AddTransient(sp => sp.GetConfigurationSection<JwtOptions>());
        serviceCollection.AddSpravyAuthentication(configuration);

        var persistKeysDirectory = "/".ToDirectory()
           .Combine("tmp")
           .Combine("spravy")
           .Combine(TAssemblyMark.AssemblyName.Name.ThrowIfNullOrWhiteSpace())
           .CreateIfNotExists();

        serviceCollection.AddDataProtection().PersistKeysToFileSystem(persistKeysDirectory);

        return serviceCollection;
    }

    public static IServiceCollection AddSpravyAuthentication(
        this IServiceCollection serviceCollection,
        IConfiguration configuration
    )
    {
        serviceCollection.AddAuthentication(x => x.SetJwtBearerDefaults())
           .AddJwtBearer(x => x.SetJwtOptions(configuration));

        return serviceCollection;
    }
}