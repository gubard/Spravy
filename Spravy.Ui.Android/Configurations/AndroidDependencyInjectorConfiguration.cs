using ExtensionFramework.Core.DependencyInjection.Interfaces;
using ExtensionFramework.Core.DependencyInjection.Extensions;
using Microsoft.Extensions.Configuration;

namespace Spravy.Ui.Android.Configurations;

public readonly struct AndroidDependencyInjectorConfiguration : IDependencyInjectorConfiguration
{
    public void Configure(IDependencyInjectorRegister register)
    {
        register.RegisterScopeDel<IConfiguration>(
            () =>
            {
                using var stream = typeof(MarkStruct).Assembly.GetManifestResourceStream("Spravy.Ui.Android.appsettings.json");

                return new ConfigurationBuilder().AddJsonStream(stream).Build();
            }
        );
    }
}