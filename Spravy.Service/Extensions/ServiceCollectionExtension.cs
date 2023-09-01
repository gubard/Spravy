using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spravy.Service.Model;

namespace Spravy.Service.Extensions;

public static class ServiceCollectionExtension
{
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
                cfg =>
                {
                    cfg.AddProfile<TProfile>();
                }
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