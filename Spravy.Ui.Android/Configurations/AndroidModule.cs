using Android.Content;
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
using Spravy.Ui.Android.Services;
using Spravy.Ui.Interfaces;
using Xamarin.Essentials;

namespace Spravy.Ui.Android.Configurations;

public class AndroidModule : NinjectModule
{
    private readonly ContextWrapper contextWrapper;

    public AndroidModule(ContextWrapper contextWrapper)
    {
        this.contextWrapper = contextWrapper;
    }

    public override void Load()
    {
        Bind<IOpenerLink>().To<AndroidOpenerLink>();
        Bind<ContextWrapper>().ToConstant(contextWrapper);

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
                {
                    using var stream = SpravyUiAndroidMark.GetResourceStream(FileNames.DefaultConfigFileName);

                    return new ConfigurationBuilder().AddJsonStream(stream.ThrowIfNull()).Build();
                }
            );

        Bind<IObjectStorage>()
            .ToConstructor(
                x => new ObjectStorage(
                    FileSystem.AppDataDirectory.ToDirectory(),
                    x.Context.Kernel.GetRequiredService<ISerializer>()
                )
            );
    }
}