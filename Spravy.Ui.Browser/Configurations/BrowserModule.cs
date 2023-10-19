using Microsoft.Extensions.Configuration;
using Ninject.Modules;
using Spravy.Authentication.Domain.Client.Models;
using Spravy.Di.Extensions;
using Spravy.Domain.Extensions;
using Spravy.Domain.Helpers;
using Spravy.EventBus.Domain.Client.Models;
using Spravy.Schedule.Domain.Client.Models;
using Spravy.ToDo.Domain.Client.Models;

namespace Spravy.Ui.Browser.Configurations;

public class BrowserModule : NinjectModule
{
    public static readonly BrowserModule Default = new();

    public override void Load()
    {
        Bind<GrpcAuthenticationServiceOptions>()
            .ToMethod(
                context => context.Kernel.GetConfigurationSection<GrpcAuthenticationServiceOptions>("GrpcRouterService")
            );

        Bind<GrpcScheduleServiceOptions>()
            .ToMethod(
                context => context.Kernel.GetConfigurationSection<GrpcScheduleServiceOptions>("GrpcRouterService")
            );

        Bind<GrpcToDoServiceOptions>()
            .ToMethod(
                context => context.Kernel.GetConfigurationSection<GrpcToDoServiceOptions>("GrpcRouterService")
            );

        Bind<GrpcEventBusServiceOptions>()
            .ToMethod(
                context => context.Kernel.GetConfigurationSection<GrpcEventBusServiceOptions>("GrpcRouterService")
            );

        Bind<IConfiguration>()
            .ToMethod(
                _ =>
                {
                    using var stream = SpravyUiBrowserMark.GetResourceStream(FileNames.DefaultConfigFileName)
                        .ThrowIfNull();

                    return new ConfigurationBuilder().AddJsonStream(stream).Build();
                }
            );
    }
}