using Android.Content;
using Microsoft.Extensions.Configuration;
using Ninject.Modules;
using Spravy.Authentication.Domain.Client.Models;
using Spravy.Db.Interfaces;
using Spravy.Db.Sqlite.EntityTypeConfigurations;
using Spravy.Db.Sqlite.Services;
using Spravy.Domain.Extensions;
using Spravy.Domain.Helpers;
using Spravy.Domain.Interfaces;
using Spravy.EventBus.Domain.Client.Models;
using Spravy.PasswordGenerator.Domain.Client.Models;
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
        Bind<IObjectStorage>().To<SqliteObjectStorage>();
        Bind<IOpenerLink>().To<AndroidOpenerLink>();
        Bind<ContextWrapper>().ToConstant(contextWrapper);

        Bind<GrpcAuthenticationServiceOptions>()
           .ToMethod(context => context.Kernel.GetConfigurationSection<GrpcAuthenticationServiceOptions>())
           .InSingletonScope();

        Bind<GrpcScheduleServiceOptions>()
           .ToMethod(context => context.Kernel.GetConfigurationSection<GrpcScheduleServiceOptions>())
           .InSingletonScope();

        Bind<GrpcToDoServiceOptions>()
           .ToMethod(context => context.Kernel.GetConfigurationSection<GrpcToDoServiceOptions>())
           .InSingletonScope();

        Bind<GrpcPasswordServiceOptions>()
           .ToMethod(context => context.Kernel.GetConfigurationSection<GrpcPasswordServiceOptions>())
           .InSingletonScope();

        Bind<GrpcEventBusServiceOptions>()
           .ToMethod(context => context.Kernel.GetConfigurationSection<GrpcEventBusServiceOptions>())
           .InSingletonScope();

        Bind<IConfiguration>()
           .ToMethod(_ =>
            {
                using var stream = SpravyUiAndroidMark.GetResourceStream(FileNames.DefaultConfigFileName);

                return new ConfigurationBuilder().AddJsonStream(stream.ThrowIfNull()).Build();
            });

        Bind<IDbContextSetup>()
           .ToMethod<SqliteDbContextSetup>(c => new(new[]
            {
                new StorageEntityTypeConfiguration(),
            }, FileSystem.AppDataDirectory.ToDirectory().ToFile("storage.db"), true));
    }
}