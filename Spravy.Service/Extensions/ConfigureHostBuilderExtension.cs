using Microsoft.AspNetCore.Builder;
using Serilog;

namespace Spravy.Service.Extensions;

public static class ConfigureHostBuilderExtension
{
    public static ConfigureHostBuilder UseSpravy(this ConfigureHostBuilder builder)
    {
        builder.UseSerilog(
            (_, _, configuration) =>
            {
                configuration.WriteTo.File("/tmp/Spravy/Spravy.ToDo.Service.log");
                configuration.WriteTo.Console();
            }
        );

        return builder;
    }
}