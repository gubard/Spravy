using System;
using Spravy.Domain.Extensions;
using Spravy.Domain.Services;
using Spravy.Ui.Android.Configurations;

namespace Spravy.Ui.Android.Modules;

public class AndroidModules : Module
{
    private const string IdString = "89c7044f-52d0-4cb2-84b9-c32597912c8d";
    private static readonly DependencyInjector MainDependencyInjector;

    public static readonly Guid IdValue = Guid.Parse(IdString);

    static AndroidModules()
    {
        var register = new DependencyInjectorRegister();
        register.RegisterConfiguration<AndroidDependencyInjectorConfiguration>();
        MainDependencyInjector = register.Build();
    }

    public AndroidModules() : base(IdValue, MainDependencyInjector)
    {
    }
}