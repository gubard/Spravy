using Microsoft.Extensions.Configuration;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;

namespace Spravy.Di.Extensions;

public static class ConfigurationExtension
{
    public static T GetConfigurationSection<T>(this IConfiguration configuration) where T : IOptionsValue
    {
        var section = configuration.GetSection(T.Section);
        Console.WriteLine($"{T.Section} {section.Value} {section.GetChildren().Select(x => x.Value).JoinString(";")}");

        return section.Get<T>().ThrowIfNull();
    }

    public static T GetConfigurationSection<T>(this IConfiguration configuration, string path) where T : IOptionsValue
    {
        var section = configuration.GetSection(path);
        Console.WriteLine($"{T.Section} {section.Value} {section.GetChildren().Select(x => x.Value).JoinString(";")}");

        return section.Get<T>().ThrowIfNull();
    }
}