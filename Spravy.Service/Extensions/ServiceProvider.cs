using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;

namespace Spravy.Service.Extensions;

public static class ServiceProvider
{
    public static T GetConfigurationSection<T>(this IServiceProvider serviceProvider, string section)
    {
        return serviceProvider.GetRequiredService<IConfiguration>().GetSection(section).Get<T>().ThrowIfNull();
    }

    public static T GetConfigurationSection<T>(this IServiceProvider serviceProvider) where T : IOptionsValue
    {
        return serviceProvider.GetRequiredService<IConfiguration>().GetSection(T.Section).Get<T>().ThrowIfNull();
    }
}