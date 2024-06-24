using Jab;
using Microsoft.Extensions.Configuration;
using Spravy.Client.Models;
using Spravy.Core.Helpers;
using Spravy.Core.Services;
using Spravy.Db.Interfaces;
using Spravy.Db.Services;
using Spravy.Db.Sqlite.EntityTypeConfigurations;
using Spravy.Db.Sqlite.Services;
using Spravy.Domain.Extensions;
using Spravy.Domain.Helpers;
using Spravy.Domain.Interfaces;
using Spravy.Ui.Android.Services;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Modules;
using Xamarin.Essentials;
using Spravy.Ui.Services;

namespace Spravy.Ui.Android.Modules;

[ServiceProvider]
[Import(typeof(IUiModule))]
[Singleton(typeof(IConfiguration), Factory = nameof(ConfigurationFactory))]
[Singleton(typeof(ClientOptions), Factory = nameof(ClientOptionsFactory))]
[Singleton(typeof(IServiceFactory), Factory = nameof(ServiceFactoryFactory))]
[Transient(typeof(IOpenerLink), typeof(AndroidOpenerLink))]
[Transient(typeof(IClipboardService), typeof(TopLevelClipboardService))]
[Transient(typeof(IDbContextSetup), Factory = nameof(DbContextSetupFactory))]
[Transient(typeof(StorageDbContext), Factory = nameof(StorageDbContextFactory))]
[Transient(typeof(IObjectStorage), Factory = nameof(SqliteObjectStorageFactory))]
public partial class AndroidServiceProvider : IServiceFactory
{
    static IObjectStorage SqliteObjectStorageFactory(StorageDbContext context, ProtobufSerializer serializer)
    {
        return new SqliteObjectStorage(context, serializer);
    }
    
    static StorageDbContext StorageDbContextFactory(IDbContextSetup setup)
    {
        return new(setup);
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
        
    public IDbContextSetup DbContextSetupFactory()
    {
        return new SqliteDbContextSetup(new[]
        {
            new StorageEntityTypeConfiguration(),
        }, FileSystem.AppDataDirectory.ToDirectory().ToFile("storage.db"), true);
    }

    public T CreateService<T>() where T : notnull
    {
        return GetService<T>();
    }
}