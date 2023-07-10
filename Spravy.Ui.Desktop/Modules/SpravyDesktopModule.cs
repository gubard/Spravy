using System;
using ExtensionFramework.Core.DependencyInjection.Services;
using ExtensionFramework.Core.DependencyInjection.Extensions;
using ExtensionFramework.Core.ModularSystem.Services;
using Spravy.Ui.Desktop.Configurations;

namespace Spravy.Ui.Desktop.Modules;

public class SpravyDesktopModule : Module
{
    private const string IdString = "89c7044f-52d0-4cb2-84b9-c32597912c8d";
    private static readonly DependencyInjector MainDependencyInjector;

    public static readonly Guid IdValue = Guid.Parse(IdString);

    static SpravyDesktopModule()
    {
        var register = new DependencyInjectorRegister();
        register.RegisterConfiguration<SpravyDesktopDependencyInjectorConfiguration>();
        MainDependencyInjector = register.Build();
    }

    public SpravyDesktopModule() : base(IdValue, MainDependencyInjector)
    {
    }
}