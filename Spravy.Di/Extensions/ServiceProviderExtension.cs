namespace Spravy.Di.Extensions;

public static class ServiceProviderExtension
{
    public static T GetConfigurationSection<T>(this IServiceProvider serviceProvider) where T : IOptionsValue
    {
        return serviceProvider.GetRequiredService<IConfiguration>().GetConfigurationSection<T>();
    }

    public static T GetConfigurationSection<T>(this IServiceProvider serviceProvider, string path)
        where T : IOptionsValue
    {
        return serviceProvider.GetRequiredService<IConfiguration>().GetConfigurationSection<T>(path);
    }
}