using Microsoft.Extensions.Configuration;
using Ninject.Modules;

namespace Spravy.Ui.Desktop.Configurations;

public class DesktopModule : NinjectModule
{
    public static readonly DesktopModule Default = new();

    public override void Load()
    {
        Bind<IConfiguration>()
            .ToMethod(
                _ =>
                    new ConfigurationBuilder().AddJsonFile("appsettings.json").Build()
            );
    }
}