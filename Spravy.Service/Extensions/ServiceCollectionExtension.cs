using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spravy.Db.Sqlite.Extensions;
using Spravy.Db.Sqlite.Models;
using Spravy.Domain.Extensions;
using Spravy.Service.Model;

namespace Spravy.Service.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddSpravySqliteFolderContext<TContext>(this IServiceCollection serviceCollection)
        where TContext : DbContext
    {
        serviceCollection.AddDbContextFactory<TContext>(
            (sp, options) =>
            {
                var sqliteOptions = sp.GetRequiredService<SqliteFolderOptions>();
                var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
                var userId = httpContextAccessor.GetUserId();
                var fileName = $"{userId}.db";
                var file = sqliteOptions.DataBasesFolder.ThrowIfNull().ToDirectory().ToFile(fileName);
                var connectionString = file.ToSqliteConnectionString();
                options.UseSqlite(connectionString);
            }
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
        serviceCollection.AddScoped(sp => sp.GetConfigurationSection<JwtOptions>(JwtOptions.Section));
        serviceCollection.AddSpravyAuthentication(configuration);

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