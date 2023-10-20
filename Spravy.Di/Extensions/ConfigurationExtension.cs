using Microsoft.Extensions.Configuration;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;

namespace Spravy.Di.Extensions;

public static class ConfigurationExtension
{
    public static T GetConfigurationSection<T>(this IConfiguration configuration) where T : IOptionsValue
    {
        return configuration.GetSection(T.Section).Get<T>().ThrowIfNull();
    }

    public static T GetConfigurationSection<T>(this IConfiguration configuration, string path) where T : IOptionsValue
    {
        return configuration.GetSection(path).Get<T>().ThrowIfNull();
    }
}