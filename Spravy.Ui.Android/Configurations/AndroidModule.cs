using Microsoft.Extensions.Configuration;
using Ninject.Modules;
using Spravy.Domain.Extensions;

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
                    using var stream =
                        typeof(MarkStruct).Assembly.GetManifestResourceStream("Spravy.Ui.Android.appsettings.json");

                    return new ConfigurationBuilder().AddJsonStream(stream.ThrowIfNull()).Build();
                }
            );
    }
}