using Avalonia.Controls;
using Jab;
using Microsoft.Extensions.Configuration;
using Spavy.LocalStorage.Sqlite.Services;
using Spravy.Client.Models;
using Spravy.Core.Helpers;
using Spravy.Domain.Extensions;
using Spravy.Domain.Helpers;
using Spravy.Domain.Interfaces;
using Spravy.Ui.Extensions;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Modules;
using Spravy.Ui.Services;

namespace Spravy.Ui.Desktop.Modules;

[ServiceProvider]
[Import(typeof(IUiModule))]
[Singleton(typeof(IConfiguration), Factory = nameof(ConfigurationFactory))]
[Singleton(typeof(ClientOptions), Factory = nameof(ClientOptionsFactory))]
[Singleton(typeof(IServiceFactory), Factory = nameof(ServiceFactoryFactory))]
[Singleton(typeof(TopLevel), Factory = nameof(TopLevelFactory))]
[Transient(typeof(IOpenerLink), typeof(OpenerLink))]
[Transient(typeof(IClipboardService), typeof(TopLevelClipboardService))]
[Transient(typeof(IObjectStorage), Factory = nameof(SqliteObjectStorageFactory))]
public partial class DesktopServiceProvider : IServiceFactory
{
    static IObjectStorage SqliteObjectStorageFactory(ISerializer serializer)
    {
        return new SqliteObjectStorage(serializer, "./storage/storage.db".ToFile());
    }

    static TopLevel TopLevelFactory(Avalonia.Application application)
    {
        return application.GetTopLevel().ThrowIfNull();
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
        return new ConfigurationBuilder().AddJsonFile(FileNames.DefaultConfigFileName).Build();
    }

    public T CreateService<T>()
        where T : notnull
    {
        return GetService<T>();
    }
}
