namespace Spravy.Service.Extensions;

public static class ConfigureHostBuilderExtension
{
    public static ConfigureHostBuilder UseSpravy(this ConfigureHostBuilder builder)
    {
        builder.UseSerilog();

        return builder;
    }
}