using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Spravy.Domain.Extensions;
using Spravy.Service.Helpers;

namespace Spravy.Service.Extensions;

public static class WebApplicationBuilderExtension
{
    public static WebApplicationBuilder AddSpravy(this WebApplicationBuilder builder, string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();

        builder.Configuration.AddSpravy(args);
        builder.Services.AddSpravy(builder.Configuration);
        builder.Host.UseSpravy();

        return builder;
    }

    public static WebApplication BuildSpravy<TService>(
        this WebApplicationBuilder builder,
        string[] args,
        Action<IServiceCollection> setupServiceCollection
    ) where TService : class
    {
        return builder.BuildSpravy<TService>(args, Enumerable.Empty<Type>(), setupServiceCollection);
    }

    public static WebApplication BuildSpravy<TService1, TService2, TService3, TService4>(
        this WebApplicationBuilder builder,
        string[] args,
        Action<IServiceCollection> setupServiceCollection
    )
        where TService1 : class
        where TService2 : class
        where TService3 : class
        where TService4 : class
    {
        return builder.BuildSpravy<TService1, TService2, TService3, TService4>(
            args,
            Enumerable.Empty<Type>(),
            setupServiceCollection
        );
    }

    public static WebApplication BuildSpravy<TService>(
        this WebApplicationBuilder builder,
        string[] args,
        Type middleware,
        Action<IServiceCollection> setupServiceCollection
    ) where TService : class
    {
        return builder.BuildSpravy<TService>(args, middleware.ToEnumerable(), setupServiceCollection);
    }

    public static WebApplication BuildSpravy<TService>(
        this WebApplicationBuilder builder,
        string[] args,
        IEnumerable<Type> middlewares,
        Action<IServiceCollection> setupServiceCollection
    ) where TService : class
    {
        builder.AddSpravy(args);
        setupServiceCollection.Invoke(builder.Services);
        var app = builder.Build();
        app.UseSerilogRequestLogging();
        app.UseRouting();
        app.UseGrpcWeb(ServiceDefaults.DefaultGrpcWebOptions);
        app.UseCors();
        app.UseAuthentication();
        app.UseAuthorization();

        foreach (var middleware in middlewares)
        {
            app.UseMiddleware(middleware);
        }

        app.MapGrpcService<TService>().EnableGrpcWeb();

        return app;
    }

    public static WebApplication BuildSpravy<TService1, TService2, TService3, TService4>(
        this WebApplicationBuilder builder,
        string[] args,
        IEnumerable<Type> middlewares,
        Action<IServiceCollection> setupServiceCollection
    )
        where TService1 : class
        where TService2 : class
        where TService3 : class
        where TService4 : class
    {
        builder.AddSpravy(args);
        setupServiceCollection.Invoke(builder.Services);

        var app = builder.Build();
        app.UseSerilogRequestLogging();
        app.UseRouting();
        app.UseGrpcWeb(ServiceDefaults.DefaultGrpcWebOptions);
        app.UseCors();
        app.UseAuthentication();
        app.UseAuthorization();

        foreach (var middleware in middlewares)
        {
            app.UseMiddleware(middleware);
        }

        app.MapGrpcService<TService1>().EnableGrpcWeb();
        app.MapGrpcService<TService2>().EnableGrpcWeb();
        app.MapGrpcService<TService3>().EnableGrpcWeb();
        app.MapGrpcService<TService4>().EnableGrpcWeb();

        return app;
    }
}