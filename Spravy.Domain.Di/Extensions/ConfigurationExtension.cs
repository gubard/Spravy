using Microsoft.Extensions.Configuration;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;

namespace Spravy.Domain.Di.Extensions;

public static class ConfigurationExtension
{
    public static T GetConfigurationSection<T>(this IConfiguration configuration) where T : IOptionsValue
    {
        return configuration.GetSection(T.Section).Get<T>().ThrowIfNull();
    }
}