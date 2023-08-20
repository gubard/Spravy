using Microsoft.Extensions.Configuration;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;

namespace Spravy.Ui.Android.Configurations;

public readonly struct AndroidDependencyInjectorConfiguration : IDependencyInjectorConfiguration
{
    public void Configure(IDependencyInjectorRegister register)
    {
        register.RegisterScope(() => CreateConfiguration());
    }

    private static IConfiguration CreateConfiguration()
    {
        using var stream = typeof(MarkStruct).Assembly.GetManifestResourceStream("Spravy.Ui.Android.appsettings.json");

        return new ConfigurationBuilder().AddJsonStream(stream.ThrowIfNull()).Build();
    }
}