using Microsoft.Extensions.Configuration;
using Ninject.Modules;

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
                    using var stream =
                        typeof(MarkStruct).Assembly.GetManifestResourceStream("Spravy.Ui.Browser.appsettings.json");

                    return new ConfigurationBuilder().AddJsonStream(stream).Build();
                }
            );
    }
}