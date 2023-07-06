using System;
using ExtensionFramework.Core.DependencyInjection.Services;
using ExtensionFramework.Core.DependencyInjection.Extensions;
using ExtensionFramework.Core.ModularSystem.Services;
using Spravy.Browser.Configurations;

namespace Spravy.Browser.Modules;

public class SpravyBrowserModule : Module
{
    private const string IdString = "eef62ce8-e24d-4159-885e-7488035909a3";
    private static readonly DependencyInjector MainDependencyInjector;

    public static readonly Guid IdValue = Guid.Parse(IdString);

    static SpravyBrowserModule()
    {
        var register = new DependencyInjectorRegister();
        register.RegisterConfiguration<SpravyBrowserDependencyInjectorConfiguration>();
        MainDependencyInjector = register.Build();
    }

    public SpravyBrowserModule() : base(IdValue, MainDependencyInjector)
    {
    }
}