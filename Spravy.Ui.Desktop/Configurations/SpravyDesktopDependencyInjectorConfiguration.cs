using ExtensionFramework.Core.DependencyInjection.Interfaces;
using ExtensionFramework.Core.DependencyInjection.Extensions;
using Microsoft.Extensions.Configuration;

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