using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Spravy.Service.Helpers;

namespace Spravy.Service.Extensions;

public static class WebApplicationBuilderExtension
{
    public static WebApplicationBuilder AddSpravy(this WebApplicationBuilder builder)
    {
        builder.Services.AddSpravy(builder.Configuration);
        builder.Host.UseSpravy();

        return builder;
    }

    public static WebApplication BuildSpravy<TService>(
        this WebApplicationBuilder builder,
        Action<IServiceCollection> setupServiceCollection
    ) where TService : class
    {
        builder.AddSpravy();
        setupServiceCollection.Invoke(builder.Services);

        var app = builder.Build();
        app.UseSerilogRequestLogging();
        app.UseRouting();
        app.UseGrpcWeb(ServiceDefaults.DefaultGrpcWebOptions);
        app.UseCors(PolicyNames.AllowAllName);
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapGrpcService<TService>().EnableGrpcWeb();

        return app;
    }
}