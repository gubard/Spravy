using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ninject.Modules;
using Spravy.Authentication.Domain.Client.Models;
using Spravy.Di.Extensions;
using Spravy.Domain.Extensions;
using Spravy.Domain.Helpers;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Services;
using Spravy.EventBus.Domain.Client.Models;
using Spravy.Schedule.Domain.Client.Models;
using Spravy.ToDo.Domain.Client.Models;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Services;

namespace Spravy.Ui.Desktop.Configurations;

public class DesktopModule : NinjectModule
{
    public static readonly DesktopModule Default = new();

    public override void Load()
    {
        Bind<IOpenerLink>().To<OpenerLink>();

        Bind<GrpcAuthenticationServiceOptions>()
            .ToMethod(context => context.Kernel.GetConfigurationSection<GrpcAuthenticationServiceOptions>());

        Bind<GrpcScheduleServiceOptions>()
            .ToMethod(context => context.Kernel.GetConfigurationSection<GrpcScheduleServiceOptions>());

        Bind<GrpcToDoServiceOptions>()
            .ToMethod(context => context.Kernel.GetConfigurationSection<GrpcToDoServiceOptions>());

        Bind<GrpcEventBusServiceOptions>()
            .ToMethod(context => context.Kernel.GetConfigurationSection<GrpcEventBusServiceOptions>());

        Bind<IConfiguration>()
            .ToMethod(
                _ =>
                    new ConfigurationBuilder().AddJsonFile(FileNames.DefaultConfigFileName).Build()
            );

        Bind<IObjectStorage>()
            .ToMethod(
                context => new FilesObjectStorage(
                    "./storage".ToDirectory(),
                    context.Kernel.GetRequiredService<ISerializer>()
                )
            );
    }
}