using System;
using Spravy.Domain.Extensions;
using Spravy.Domain.Services;
using Spravy.Ui.Configurations;

namespace Spravy.Ui.Modules;

public class SpravyModule : Module
{
    private const string IdString = "78f99dc5-e564-433f-a82f-0bb48dff3ea1";
    private static readonly DependencyInjector MainDependencyInjector;

    public static readonly Guid IdValue = Guid.Parse(IdString);

    static SpravyModule()
    {
        var register = new DependencyInjectorRegister();
        register.RegisterConfiguration<SpravyDependencyInjectorConfiguration>();
        MainDependencyInjector = register.Build();
    }

    public SpravyModule() : base(IdValue, MainDependencyInjector)
    {
    }
}