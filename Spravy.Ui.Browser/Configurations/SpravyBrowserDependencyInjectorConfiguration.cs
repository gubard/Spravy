using Microsoft.Extensions.Configuration;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;

namespace Spravy.Ui.Browser.Configurations;

public readonly struct SpravyBrowserDependencyInjectorConfiguration : IDependencyInjectorConfiguration
{
    public void Configure(IDependencyInjectorRegister register)
    {
        register.RegisterScopeDel<IConfiguration>(
            () =>
            {
                using var stream = typeof(MarkStruct).Assembly.GetManifestResourceStream("Spravy.Ui.Browser.appsettings.json");

                return new ConfigurationBuilder().AddJsonStream(stream).Build();
            }
        );
    }
}