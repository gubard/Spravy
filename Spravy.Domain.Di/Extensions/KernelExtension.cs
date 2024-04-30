using Microsoft.Extensions.Configuration;
using Ninject;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;

namespace Spravy.Domain.Di.Extensions;

public static class KernelExtension
{
    public static TOption GetOptionsValue<TOption>(this IKernel kernel) where TOption : IOptionsValue
    {
        return kernel.Get<IConfiguration>().GetRequiredSection(TOption.Section).Get<TOption>().ThrowIfNull();
    }
}