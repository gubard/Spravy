namespace Spravy.Integration.Tests.Configurations;

public class TestModule : NinjectModule
{
    public static readonly TestModule Default = new();

    public override void Load()
    {
        Bind<IObjectStorage>().To<SqliteObjectStorage>();
        Bind<IOpenerLink>().To<OpenerLink>();

        Bind<GrpcPasswordServiceOptions>()
           .ToMethod(context => context.Kernel.GetConfigurationSection<GrpcPasswordServiceOptions>())
           .InSingletonScope();

        Bind<GrpcAuthenticationServiceOptions>()
           .ToMethod(context => context.Kernel.GetConfigurationSection<GrpcAuthenticationServiceOptions>())
           .InSingletonScope();

        Bind<GrpcScheduleServiceOptions>()
           .ToMethod(context => context.Kernel.GetConfigurationSection<GrpcScheduleServiceOptions>())
           .InSingletonScope();

        Bind<GrpcToDoServiceOptions>()
           .ToMethod(context => context.Kernel.GetConfigurationSection<GrpcToDoServiceOptions>())
           .InSingletonScope();

        Bind<GrpcEventBusServiceOptions>()
           .ToMethod(context => context.Kernel.GetConfigurationSection<GrpcEventBusServiceOptions>())
           .InSingletonScope();

        Bind<IConfiguration>()
           .ToMethod(_ => new ConfigurationBuilder().AddJsonFile("testsettings.json").Build());

        Bind<IDbContextSetup>()
           .ToMethod<SqliteDbContextSetup>(_ => new(new[]
            {
                new StorageEntityTypeConfiguration(),
            }, "./storage/storage.db".ToFile(), true));
    }
}