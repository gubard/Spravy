using Spravy.Authentication.Domain.Interfaces;
using Spravy.Authentication.Domain.Services;

namespace Spravy.Ui.Android.Modules;

[ServiceProvider]
[Import(typeof(IUiModule))]
[Singleton(typeof(IFactory<ISingleViewTopLevelControl>), typeof(AndroidSingleViewTopLevelControlFactory))]
[Singleton(typeof(IConfigurationLoader), typeof(EmbeddedConfigurationLoader))]
[Singleton(typeof(IConfiguration), Factory = nameof(ConfigurationFactory))]
[Singleton(typeof(ClientOptions), Factory = nameof(ClientOptionsFactory))]
[Singleton(typeof(IServiceFactory), Factory = nameof(ServiceFactoryFactory))]
[Singleton(typeof(ISoundPlayer), typeof(SoundPlayer))]
[Transient(typeof(IObjectStorage), Factory = nameof(SqliteObjectStorageFactory))]
[Transient(typeof(IOpenerLink), typeof(AndroidOpenerLink))]
[Transient(typeof(IHashService), typeof(Md5HashService))]
[Transient(typeof(IClipboardService), typeof(AvaloniaClipboardService))]
public partial class AndroidServiceProvider : IServiceFactory
{
    public T CreateService<T>() where T : notnull
    {
        return GetService<T>();
    }

    private static IObjectStorage SqliteObjectStorageFactory(ISerializer serializer)
    {
        return new SqliteObjectStorage(serializer, FileSystem.AppDataDirectory.ToDirectory().ToFile("storage.db"));
    }

    public IServiceFactory ServiceFactoryFactory()
    {
        return DiHelper.ServiceFactory;
    }

    public ClientOptions ClientOptionsFactory()
    {
        return new(true);
    }

    public IConfiguration ConfigurationFactory()
    {
        using var stream = SpravyUiAndroidMark.GetResourceStream(FileNames.DefaultConfigFileName);

        return new ConfigurationBuilder().AddJsonStream(stream.ThrowIfNull()).Build();
    }
}