using System.Reflection;
using Spravy.Domain.Interfaces;

namespace Spravy.Domain.Extensions;

public static class DependencyInjectorRegisterExtension
{
    public static void RegisterConfigurationFromAssemblies<TDependencyInjectorRegister>(
        this TDependencyInjectorRegister register
    )
        where TDependencyInjectorRegister : IDependencyInjectorRegister
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        register.RegisterConfigurationFromAssemblies(assemblies);
    }

    public static void RegisterConfigurationFromAssemblies<TDependencyInjectorRegister>(
        this TDependencyInjectorRegister register,
        IEnumerable<Assembly> assemblies
    )
        where TDependencyInjectorRegister : IDependencyInjectorRegister
    {
        foreach (var assembly in assemblies)
        {
            register.RegisterConfigurationFromAssembly(assembly);
        }
    }

    public static void RegisterConfigurationFromAssembly<TDependencyInjectorRegister>(
        this TDependencyInjectorRegister register,
        Assembly assembly
    )
        where TDependencyInjectorRegister : IDependencyInjectorRegister
    {
        var types = assembly.GetTypes();
        var dependencyInjectorConfigurations = types
            .Where(
                x =>
                    x is { IsInterface: false, IsAbstract: false }
                    && typeof(IDependencyInjectorConfiguration).IsAssignableFrom(x)
            )
            .ToArray();

        foreach (var type in dependencyInjectorConfigurations)
        {
            var dependencyInjectorConfiguration = Activator
                .CreateInstance(type)
                .ThrowIfNull()
                .ThrowIfIsNot<IDependencyInjectorConfiguration>();

            register.RegisterConfiguration(dependencyInjectorConfiguration);
        }
    }

    public static void RegisterConfiguration<TConfiguration>(
        this IDependencyInjectorRegister register
    )
        where TConfiguration : IDependencyInjectorConfiguration, new()
    {
        new TConfiguration().Configure(register);
    }
}