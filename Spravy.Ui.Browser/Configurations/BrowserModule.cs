using Microsoft.Extensions.Configuration;
using Ninject.Modules;
using Spravy.Domain.Helpers;

namespace Spravy.Ui.Browser.Configurations;

public class BrowserModule : NinjectModule
{
    public static readonly BrowserModule Default = new();

    public override void Load()
    {
        Bind<IConfiguration>()
            .ToMethod(
                _ =>
                {
                    using var stream = SpravyUiBrowserMark.GetResourceStream(FileNames.DefaultConfigFileName);

                    return new ConfigurationBuilder().AddJsonStream(stream).Build();
                }
            );
    }
}