using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Serilog;
using Spravy.Domain.Extensions;

namespace Spravy.Service.Extensions;

public static class ConfigureHostBuilderExtension
{
    public static ConfigureHostBuilder UseSpravy(this ConfigureHostBuilder builder)
    {
        builder.UseSerilog(
            (_, _, configuration) =>
            {
                configuration.WriteTo.File(
                    $"/tmp/Spravy/{Assembly.GetEntryAssembly().ThrowIfNull().GetName().Name}.log"
                );
                configuration.WriteTo.Console();
            }
        );

        return builder;
    }
}