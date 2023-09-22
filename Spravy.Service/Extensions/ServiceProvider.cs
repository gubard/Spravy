using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spravy.Domain.Di.Extensions;
using Spravy.Domain.Interfaces;

namespace Spravy.Service.Extensions;

public static class ServiceProvider
{
    public static T GetConfigurationSection<T>(this IServiceProvider serviceProvider) where T : IOptionsValue
    {
        return serviceProvider.GetRequiredService<IConfiguration>().GetConfigurationSection<T>();
    }
}