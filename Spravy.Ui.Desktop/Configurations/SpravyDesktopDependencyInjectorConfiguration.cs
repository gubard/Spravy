using Microsoft.Extensions.Configuration;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;

namespace Spravy.Ui.Desktop.Configurations;

public readonly struct SpravyDesktopDependencyInjectorConfiguration : IDependencyInjectorConfiguration
{
    public void Configure(IDependencyInjectorRegister register)
    {
        register.RegisterSingleton<IConfiguration>(
            () =>
                new ConfigurationBuilder().AddJsonFile("appsettings.json").Build()
        );
    }
}