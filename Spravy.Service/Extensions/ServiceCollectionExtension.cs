using AutoMapper;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spravy.Db.Contexts;
using Spravy.Db.Interfaces;
using Spravy.Db.Services;
using Spravy.Db.Sqlite.Services;
using Spravy.Di.Extensions;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Services;
using Spravy.Service.Model;
using Spravy.Service.Services;

namespace Spravy.Service.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddSpravySqliteFolderContext<TDbContext, TAssemblyMark>(
        this IServiceCollection serviceCollection
    )
        where TDbContext : SpravyDbContext, IDbContextCreator<TDbContext>
        where TAssemblyMark : IAssemblyMark
    {
        serviceCollection.AddTransient<IFactory<TDbContext>, SqliteDbContextFactory<TDbContext>>();
        serviceCollection.AddTransient<DbContextFactory<TDbContext, TAssemblyMark>>();
        serviceCollection.AddTransient<ICacheValidator<string, TDbContext>, DbContextCacheValidator<TDbContext>>();

        serviceCollection.AddTransient<IFactory<string, TDbContext>>(
            sp => new CacheFactory<string, TDbContext>(
                sp.GetRequiredService<DbContextFactory<TDbContext, TAssemblyMark>>(),
                sp.GetRequiredService<ICacheValidator<string, TDbContext>>()
            )
        );

        return serviceCollection;
    }

    public static IServiceCollection AddSpravy(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddGrpc();
        serviceCollection.AddAuthorization();
        serviceCollection.AddHttpContextAccessor();
        serviceCollection.AddSingleton<IMapper>(sp => new Mapper(sp.GetRequiredService<MapperConfiguration>()));
        serviceCollection.AddCors(o => o.AddAllowAllPolicy());
        serviceCollection.AddTransient(sp => sp.GetConfigurationSection<JwtOptions>());
        serviceCollection.AddSpravyAuthentication(configuration);

        serviceCollection.AddDataProtection()
            .PersistKeysToFileSystem(Path.Combine("tmp", "Sparvy", Guid.NewGuid().ToString()).ToDirectory());

        return serviceCollection;
    }

    public static IServiceCollection AddMapperConfiguration<TProfile1, TProfile2, TProfile3>(
        this IServiceCollection serviceCollection
    )
        where TProfile1 : Profile, new()
        where TProfile2 : Profile, new()
        where TProfile3 : Profile, new()
    {
        serviceCollection.AddSingleton(
            _ => new MapperConfiguration(
                cfg =>
                {
                    cfg.AddProfile<TProfile1>();
                    cfg.AddProfile<TProfile2>();
                    cfg.AddProfile<TProfile3>();
                }
            )
        );

        return serviceCollection;
    }

    public static IServiceCollection AddMapperConfiguration<TProfile1, TProfile2, TProfile3, TProfile4>(
        this IServiceCollection serviceCollection
    )
        where TProfile1 : Profile, new()
        where TProfile2 : Profile, new()
        where TProfile3 : Profile, new()
        where TProfile4 : Profile, new()
    {
        serviceCollection.AddSingleton(
            _ => new MapperConfiguration(
                cfg =>
                {
                    cfg.AddProfile<TProfile1>();
                    cfg.AddProfile<TProfile2>();
                    cfg.AddProfile<TProfile3>();
                    cfg.AddProfile<TProfile4>();
                }
            )
        );

        return serviceCollection;
    }

    public static IServiceCollection AddMapperConfiguration<TProfile1, TProfile2>(
        this IServiceCollection serviceCollection
    )
        where TProfile1 : Profile, new()
        where TProfile2 : Profile, new()
    {
        serviceCollection.AddSingleton(
            _ => new MapperConfiguration(
                cfg =>
                {
                    cfg.AddProfile<TProfile1>();
                    cfg.AddProfile<TProfile2>();
                }
            )
        );

        return serviceCollection;
    }

    public static IServiceCollection AddMapperConfiguration<TProfile>(
        this IServiceCollection serviceCollection
    )
        where TProfile : Profile, new()
    {
        serviceCollection.AddSingleton(
            _ => new MapperConfiguration(
                cfg => { cfg.AddProfile<TProfile>(); }
            )
        );

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