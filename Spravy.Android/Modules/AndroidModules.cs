using System;
using ExtensionFramework.Core.DependencyInjection.Services;
using ExtensionFramework.Core.ModularSystem.Services;

namespace Spravy.Android.Modules;

public class AndroidModules : Module
{
    private const string IdString = "89c7044f-52d0-4cb2-84b9-c32597912c8d";
    private static readonly DependencyInjector MainDependencyInjector;

    public static readonly Guid IdValue = Guid.Parse(IdString);

    static AndroidModules()
    {
        var register = new DependencyInjectorRegister();
        MainDependencyInjector = register.Build();
    }

    public AndroidModules() : base(IdValue, MainDependencyInjector)
    {
    }
}