using Microsoft.Extensions.Configuration;

namespace Spravy.Core.Extensions;

public static class ServiceFactoryExtension
{
    public static TOption GetOptionsValue<TOption>(this IServiceFactory factory) where TOption : IOptionsValue
    {
        return factory.CreateService<IConfiguration>().GetRequiredSection(TOption.Section).Get<TOption>().ThrowIfNull();
    }
}