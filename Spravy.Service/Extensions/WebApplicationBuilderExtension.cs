using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Spravy.Client.Extensions;
using Spravy.Core.Services;
using Spravy.Domain.Errors;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Service.Helpers;

namespace Spravy.Service.Extensions;

public static class WebApplicationBuilderExtension
{
    public static WebApplicationBuilder AddSpravy<TAssemblyMark>(this WebApplicationBuilder builder, string[] args)
        where TAssemblyMark : IAssemblyMark
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();

        builder.Configuration.AddSpravy(args);
        builder.Services.AddSpravy<TAssemblyMark>(builder.Configuration);
        builder.Host.UseSpravy();

        return builder;
    }

    public static WebApplication BuildSpravy<TService, TAssemblyMark>(
        this WebApplicationBuilder builder,
        string[] args,
        Action<IServiceCollection> setupServiceCollection
    ) where TService : class where TAssemblyMark : IAssemblyMark
    {
        return builder.BuildSpravy<TService, TAssemblyMark>(args, Enumerable.Empty<Type>(), setupServiceCollection);
    }

    public static WebApplication BuildSpravy<TService1, TService2, TAssemblyMark>(
        this WebApplicationBuilder builder,
        string[] args,
        Action<IServiceCollection> setupServiceCollection
    ) where TService1 : class where TService2 : class where TAssemblyMark : IAssemblyMark
    {
        return builder.BuildSpravy<TService1, TService2, TAssemblyMark>(
            args,
            Enumerable.Empty<Type>(),
            setupServiceCollection
        );
    }

    public static WebApplication BuildSpravy<TService1, TService2, TService3, TService4, TAssemblyMark>(
        this WebApplicationBuilder builder,
        string[] args,
        Action<IServiceCollection> setupServiceCollection
    )
        where TService1 : class
        where TService2 : class
        where TService3 : class
        where TService4 : class
        where TAssemblyMark : IAssemblyMark
    {
        return builder.BuildSpravy<TService1, TService2, TService3, TService4, TAssemblyMark>(
            args,
            Enumerable.Empty<Type>(),
            setupServiceCollection
        );
    }

    public static WebApplication BuildSpravy<TService, TAssemblyMark>(
        this WebApplicationBuilder builder,
        string[] args,
        Type middleware,
        Action<IServiceCollection> setupServiceCollection
    ) where TService : class where TAssemblyMark : IAssemblyMark
    {
        return builder.BuildSpravy<TService, TAssemblyMark>(args, middleware.ToEnumerable(), setupServiceCollection);
    }

    public static WebApplication BuildSpravy<TService, TAssemblyMark>(
        this WebApplicationBuilder builder,
        string[] args,
        IEnumerable<Type> middlewares,
        Action<IServiceCollection> setupServiceCollection
    ) where TService : class where TAssemblyMark : IAssemblyMark
    {
        builder.AddSpravy<TAssemblyMark>(args);
        setupServiceCollection.Invoke(builder.Services);
        var app = builder.Build();
        app.UseSerilogRequestLogging();

        if (app.Environment.IsProduction())
        {
            app.UseHsts();
        }

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

    public static WebApplication BuildSpravy<TService1, TService2, TAssemblyMark>(
        this WebApplicationBuilder builder,
        string[] args,
        IEnumerable<Type> middlewares,
        Action<IServiceCollection> setupServiceCollection
    ) where TService1 : class where TService2 : class where TAssemblyMark : IAssemblyMark
    {
        builder.AddSpravy<TAssemblyMark>(args);
        setupServiceCollection.Invoke(builder.Services);
        var app = builder.Build();
        app.UseSerilogRequestLogging();

        if (app.Environment.IsProduction())
        {
            app.UseHsts();
        }

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

        return app;
    }

    public static WebApplication BuildSpravy<TService1, TService2, TService3, TService4, TAssemblyMark>(
        this WebApplicationBuilder builder,
        string[] args,
        IEnumerable<Type> middlewares,
        Action<IServiceCollection> setupServiceCollection
    )
        where TService1 : class
        where TService2 : class
        where TService3 : class
        where TService4 : class
        where TAssemblyMark : IAssemblyMark
    {
        builder.AddSpravy<TAssemblyMark>(args);
        setupServiceCollection.Invoke(builder.Services);

        var app = builder.Build();
        app.UseSerilogRequestLogging();

        if (app.Environment.IsProduction())
        {
            app.UseHsts();
        }

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