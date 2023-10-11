using Microsoft.Extensions.Configuration;
using Ninject.Modules;
using Spravy.Domain.Extensions;
using Spravy.Domain.Helpers;

namespace Spravy.Ui.Android.Configurations;

public class AndroidModule : NinjectModule
{
    public static readonly AndroidModule Default = new();

    public override void Load()
    {
        Bind<IConfiguration>()
            .ToMethod(
                _ =>
                {
                    using var stream = SpravyUiAndroidMark.GetResourceStream(FileNames.DefaultConfigFileName);

                    return new ConfigurationBuilder().AddJsonStream(stream.ThrowIfNull()).Build();
                }
            );
    }
}