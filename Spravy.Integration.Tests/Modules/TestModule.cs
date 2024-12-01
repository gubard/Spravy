using Jab;
using Spavy.LocalStorage.Sqlite.Services;
using Spravy.Client.Models;
using Spravy.Core.Helpers;
using Spravy.Integration.Tests.Services;
using Spravy.Ui.Desktop.Services;
using Spravy.Ui.Modules;

namespace Spravy.Integration.Tests.Modules;

[ServiceProvider]
[Import(typeof(IUiModule))]
[Singleton(typeof(IConfigurationLoader), typeof(FileConfigurationLoader))]
[Singleton(typeof(IConfiguration), Factory = nameof(ConfigurationFactory))]
[Singleton(typeof(ClientOptions), Factory = nameof(ClientOptionsFactory))]
[Singleton(typeof(IServiceFactory), Factory = nameof(ServiceFactoryFactory))]
[Singleton(typeof(TopLevel), Factory = nameof(TopLevelFactory))]
[Transient(typeof(IOpenerLink), typeof(OpenerLink))]
[Transient(typeof(ISoundPlayer), typeof(EmptySoundPlayer))]
[Transient(typeof(IClipboardService), typeof(CodeClipboardService))]
[Transient(typeof(IObjectStorage), Factory = nameof(SqliteObjectStorageFactory))]
public partial class TestServiceProvider : IServiceFactory
{
    public T CreateService<T>() where T : notnull
    {
        return GetService<T>();
    }

    private static IObjectStorage SqliteObjectStorageFactory(ISerializer serializer)
    {
        return new SqliteObjectStorage(serializer, "./storage/storage.db".ToFile());
    }

    private static TopLevel TopLevelFactory(IDesktopTopLevelControl desktopTopLevelControl)
    {
        return desktopTopLevelControl.ThrowIfIsNotCast<Window>();
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
        return new ConfigurationBuilder().AddJsonFile("testsettings.json").Build();
    }
}