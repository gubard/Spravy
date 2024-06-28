using Microsoft.Extensions.Configuration;

namespace Spravy.Core.Extensions;

public static class ConfigurationExtension
{
    public static TOption GetOptionsValue<TOption>(this IConfiguration configuration)
        where TOption : IOptionsValue
    {
        return configuration.GetRequiredSection(TOption.Section).Get<TOption>().ThrowIfNull();
    }

    public static T GetConfigurationSection<T>(this IConfiguration configuration)
        where T : IOptionsValue
    {
        var section = configuration.GetSection(T.Section);

        return section.Get<T>().ThrowIfNull();
    }

    public static T GetConfigurationSection<T>(this IConfiguration configuration, string path)
        where T : IOptionsValue
    {
        var section = configuration.GetSection(path);

        return section.Get<T>().ThrowIfNull();
    }
}
