using Microsoft.Extensions.Configuration;
using Ninject.Modules;
using Spravy.Authentication.Domain.Client.Models;
using Spravy.Domain.Extensions;
using Spravy.Domain.Helpers;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Services;
using Spravy.EventBus.Domain.Client.Models;
using Spravy.PasswordGenerator.Domain.Client.Models;
using Spravy.Schedule.Domain.Client.Models;
using Spravy.ToDo.Domain.Client.Models;
using Spravy.Ui.Browser.Services;
using Spravy.Ui.Interfaces;

namespace Spravy.Ui.Browser.Configurations;

public class BrowserModule : NinjectModule
{
    public static readonly BrowserModule Default = new();

    public override void Load()
    {
        Bind<IOpenerLink>().To<BrowserOpenerLink>();

        Bind<GrpcPasswordServiceOptions>()
           .ToMethod(context =>
                context.Kernel.GetConfigurationSection<GrpcPasswordServiceOptions>("GrpcRouterService"))
           .InSingletonScope();

        Bind<GrpcAuthenticationServiceOptions>()
           .ToMethod(context =>
                context.Kernel.GetConfigurationSection<GrpcAuthenticationServiceOptions>("GrpcRouterService"))
           .InSingletonScope();

        Bind<GrpcScheduleServiceOptions>()
           .ToMethod(context => context.Kernel.GetConfigurationSection<GrpcScheduleServiceOptions>("GrpcRouterService"))
           .InSingletonScope();

        Bind<GrpcToDoServiceOptions>()
           .ToMethod(context => context.Kernel.GetConfigurationSection<GrpcToDoServiceOptions>("GrpcRouterService"))
           .InSingletonScope();

        Bind<GrpcEventBusServiceOptions>()
           .ToMethod(context => context.Kernel.GetConfigurationSection<GrpcEventBusServiceOptions>("GrpcRouterService"))
           .InSingletonScope();

        Bind<IConfiguration>()
           .ToMethod(_ =>
            {
                using var stream = SpravyUiBrowserMark.GetResourceStream(FileNames.DefaultConfigFileName).ThrowIfNull();

                return new ConfigurationBuilder().AddJsonStream(stream).Build();
            });

        Bind<IObjectStorage>().To<LocalStorageObjectStorage>();
        Bind<IStringToBytes>().To<StringToUtf8Bytes>();
        Bind<IBytesToString>().To<Utf8BytesToString>();
    }
}