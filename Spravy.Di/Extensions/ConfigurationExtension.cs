using Microsoft.Extensions.Configuration;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;

namespace Spravy.Di.Extensions;

public static class ConfigurationExtension
{
    public static T GetConfigurationSection<T>(this IConfiguration configuration) where T : IOptionsValue
    {
        Console.WriteLine($"{T.Section} {configuration.GetSection(T.Section).Value}");

        return configuration.GetSection(T.Section).Get<T>().ThrowIfNull();
    }

    public static T GetConfigurationSection<T>(this IConfiguration configuration, string path) where T : IOptionsValue
    {
        Console.WriteLine($"{path} {configuration.GetSection(path).Value}");

        return configuration.GetSection(path).Get<T>().ThrowIfNull();
    }
}