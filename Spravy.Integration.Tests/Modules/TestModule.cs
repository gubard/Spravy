using Jab;
using Spravy.Client.Models;
using Spravy.Core.Helpers;
using Spravy.Db.Services;
using Spravy.Ui.Modules;

namespace Spravy.Integration.Tests.Modules;

[ServiceProvider]
[Import(typeof(IUiModule))]
[Singleton(typeof(IConfiguration), Factory = nameof(ConfigurationFactory))]
[Singleton(typeof(ClientOptions), Factory = nameof(ClientOptionsFactory))]
[Singleton(typeof(IServiceFactory), Factory = nameof(ServiceFactoryFactory))]
[Transient(typeof(IObjectStorage), typeof(SqliteObjectStorage))]
[Transient(typeof(IOpenerLink), typeof(OpenerLink))]
[Transient(typeof(IDbContextSetup), Factory = nameof(DbContextSetupFactory))]
[Transient(typeof(StorageDbContext), Factory = nameof(StorageDbContextFactory))]
[Transient(typeof(IClipboardService), typeof(CodeClipboardService))]
public partial class TestServiceProvider : IServiceFactory
{
    private readonly IServiceProvider serviceProvider;
    
    public TestServiceProvider()
    {
        serviceProvider = this;
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
    
    public object CreateService(Type type)
    {
        return serviceProvider.GetService(type).ThrowIfNull();
    }
}