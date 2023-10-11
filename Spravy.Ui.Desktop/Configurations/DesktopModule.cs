using Microsoft.Extensions.Configuration;
using Ninject.Modules;
using Spravy.Domain.Helpers;

namespace Spravy.Ui.Desktop.Configurations;

public class DesktopModule : NinjectModule
{
    public static readonly DesktopModule Default = new();

    public override void Load()
    {
        Bind<IConfiguration>()
            .ToMethod(
                _ =>
                    new ConfigurationBuilder().AddJsonFile(FileNames.DefaultConfigFileName).Build()
            );
    }
}