using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ninject.Modules;
using Spravy.Authentication.Domain.Client.Models;
using Spravy.Core.Services;
using Spravy.Di.Extensions;
using Spravy.Domain.Extensions;
using Spravy.Domain.Helpers;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Services;
using Spravy.EventBus.Domain.Client.Models;
using Spravy.Schedule.Domain.Client.Models;
using Spravy.ToDo.Domain.Client.Models;

namespace Spravy.Ui.Desktop.Configurations;

public class DesktopModule : NinjectModule
{
    public static readonly DesktopModule Default = new();

    public override void Load()
    {
        Bind<ISerializer>().To<ProtobufSerializer>();

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
            .ToConstructor(
                x => new ObjectStorage("./storage".ToDirectory(), x.Context.Kernel.GetRequiredService<ISerializer>())
            );
    }
}