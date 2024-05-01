using Microsoft.Extensions.Configuration;
using Ninject.Modules;
using Spravy.Authentication.Domain.Client.Models;
using Spravy.Db.Interfaces;
using Spravy.Db.Sqlite.EntityTypeConfigurations;
using Spravy.Db.Sqlite.Services;
using Spravy.Di.Extensions;
using Spravy.Domain.Extensions;
using Spravy.Domain.Helpers;
using Spravy.Domain.Interfaces;
using Spravy.EventBus.Domain.Client.Models;
using Spravy.PasswordGenerator.Domain.Client.Models;
using Spravy.Schedule.Domain.Client.Models;
using Spravy.ToDo.Domain.Client.Models;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Services;

namespace Spravy.Integration.Tests.Configurations;

public class TestModule : NinjectModule
{
    public static readonly TestModule Default = new();

    public override void Load()
    {
        Bind<IObjectStorage>().To<SqliteObjectStorage>();
        Bind<IOpenerLink>().To<OpenerLink>();

        Bind<GrpcPasswordServiceOptions>()
           .ToMethod(context => context.Kernel.GetConfigurationSection<GrpcPasswordServiceOptions>("GrpcRouterService"))
           .InSingletonScope();

        Bind<GrpcAuthenticationServiceOptions>()
           .ToMethod(context => context.Kernel.GetConfigurationSection<GrpcAuthenticationServiceOptions>("GrpcRouterService"))
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
           .ToMethod(_ => new ConfigurationBuilder().AddJsonFile(FileNames.DefaultConfigFileName).Build());

        Bind<IDbContextSetup>()
           .ToMethod<SqliteDbContextSetup>(c => new(new[]
            {
                new StorageEntityTypeConfiguration(),
            }, "./storage/storage.db".ToFile(), true));
    }
}