using Jab;
using Spravy.Client.Models;
using Spravy.Core.Helpers;
using Spravy.Db.Services;
using Spravy.Ui.Modules;
using Spravy.Core.Services;

namespace Spravy.Integration.Tests.Modules;

[ServiceProvider]
[Import(typeof(IUiModule))]
[Singleton(typeof(IConfiguration), Factory = nameof(ConfigurationFactory))]
[Singleton(typeof(ClientOptions), Factory = nameof(ClientOptionsFactory))]
[Singleton(typeof(IServiceFactory), Factory = nameof(ServiceFactoryFactory))]
[Singleton(typeof(TopLevel), Factory = nameof(TopLevelFactory))]
[Transient(typeof(IOpenerLink), typeof(OpenerLink))]
[Transient(typeof(IClipboardService), typeof(CodeClipboardService))]
[Transient(typeof(IDbContextSetup), Factory = nameof(DbContextSetupFactory))]
[Transient(typeof(StorageDbContext), Factory = nameof(StorageDbContextFactory))]
[Transient(typeof(IObjectStorage), Factory = nameof(SqliteObjectStorageFactory))]
public partial class TestServiceProvider : IServiceFactory
{
    static TopLevel TopLevelFactory(IDesktopTopLevelControl desktopTopLevelControl)
    {
        return desktopTopLevelControl.ThrowIfIsNotCast<Window>();
    }
    
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
        return new ConfigurationBuilder().AddJsonFile("testsettings.json").Build();
    }
    
    public IDbContextSetup DbContextSetupFactory()
    {
        return new SqliteDbContextSetup(new[]
        {
            new StorageEntityTypeConfiguration(),
        }, "./storage/storage.db".ToFile(), true);
    }
    
    public T CreateService<T>() where T : notnull
    {
        return GetService<T>();
    }
}