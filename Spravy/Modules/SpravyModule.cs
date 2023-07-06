using System;
using ExtensionFramework.AvaloniaUi.Configurations;
using ExtensionFramework.Core.DependencyInjection.Extensions;
using ExtensionFramework.Core.DependencyInjection.Services;
using ExtensionFramework.Core.ModularSystem.Services;
using ExtensionFramework.ReactiveUI.Configurations;
using Spravy.Configurations;

namespace Spravy.Modules;

public class SpravyModule : Module
{
    private const string IdString = "78f99dc5-e564-433f-a82f-0bb48dff3ea1";
    private static readonly DependencyInjector MainDependencyInjector;

    public static readonly Guid IdValue = Guid.Parse(IdString);

    static SpravyModule()
    {
        var register = new DependencyInjectorRegister();
        register.RegisterConfiguration<SpravyDependencyInjectorConfiguration>();
        register.RegisterConfiguration<AvaloniaUiDependencyInjectorConfiguration>();
        register.RegisterConfiguration<ReactiveUIDependencyInjectorConfiguration>();
        MainDependencyInjector = register.Build();
    }

    public SpravyModule() : base(IdValue, MainDependencyInjector)
    {
    }
}