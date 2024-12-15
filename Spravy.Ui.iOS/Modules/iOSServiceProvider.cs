using Jab;
using Microsoft.Extensions.Configuration;
using Spavy.LocalStorage.Sqlite.Services;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Authentication.Domain.Services;
using Spravy.Client.Models;
using Spravy.Core.Helpers;
using Spravy.Domain.Extensions;
using Spravy.Domain.Helpers;
using Spravy.Domain.Interfaces;
using Spravy.Ui.Interfaces;
using Spravy.Ui.iOS.Services;
using Spravy.Ui.Modules;
using Spravy.Ui.Services;

namespace Spravy.Ui.iOS.Modules;

[ServiceProvider]
[Import(typeof(IUiModule))]
[Singleton(typeof(IFactory<ISingleViewTopLevelControl>), typeof(iOSSingleViewTopLevelControlFactory))]
[Singleton(typeof(IConfigurationLoader), typeof(EmbeddedConfigurationLoader))]
[Singleton(typeof(IConfiguration), Factory = nameof(ConfigurationFactory))]
[Singleton(typeof(ClientOptions), Factory = nameof(ClientOptionsFactory))]
[Singleton(typeof(IServiceFactory), Factory = nameof(ServiceFactoryFactory))]
[Singleton(typeof(ISoundPlayer), typeof(EmptySoundPlayer))]
[Transient(typeof(IObjectStorage), Factory = nameof(SqliteObjectStorageFactory))]
[Transient(typeof(IOpenerLink), typeof(iOSOpenerLink))]
[Transient(typeof(IHashService), typeof(Md5HashService))]
[Transient(typeof(IClipboardService), typeof(AvaloniaClipboardService))]
public partial class iOSServiceProvider : IServiceFactory
{
    public T CreateService<T>() where T : notnull
    {
        return GetService<T>();
    }

    private static IObjectStorage SqliteObjectStorageFactory(ISerializer serializer)
    {
        return new SqliteObjectStorage(serializer, Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).ToDirectory().ToFile("storage.db"));
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
        using var stream = SpravyUiiOSMark.GetResourceStream(FileNames.DefaultConfigFileName);

        return new ConfigurationBuilder().AddJsonStream(stream.ThrowIfNull()).Build();
    }
}