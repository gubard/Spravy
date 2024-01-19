using Spravy.Db.Sqlite.EntityTypeConfigurations;
using Spravy.Db.Sqlite.Services;
using Spravy.Ui.Android.Services;
using Spravy.Ui.Interfaces;

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

        Bind<IDbContextSetup>()
            .ToMethod<SqliteDbContextSetup>(
                c => new SqliteDbContextSetup(
                    new[]
                    {
                        new StorageEntityTypeConfiguration()
                    },
                    FileSystem.AppDataDirectory.ToDirectory().ToFile("storage.db"),
                    true
                )
            );
    }
}