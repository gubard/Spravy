using System;
using Jab;
using Microsoft.Extensions.Configuration;
using Spravy.Client.Models;
using Spravy.Core.Helpers;
using Spravy.Domain.Extensions;
using Spravy.Domain.Helpers;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Services;
using Spravy.Ui.Browser.Services;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Modules;
using Spravy.Ui.Services;

namespace Spravy.Ui.Browser.Modules;

[ServiceProvider]
[Import(typeof(IUiModule))]
[Singleton(typeof(IConfiguration), Factory = nameof(ConfigurationFactory))]
[Singleton(typeof(ClientOptions), Factory = nameof(ClientOptionsFactory))]
[Singleton(typeof(IServiceFactory), Factory = nameof(ServiceFactoryFactory))]
[Transient(typeof(IObjectStorage), typeof(LocalStorageObjectStorage))]
[Transient(typeof(IStringToBytes), typeof(StringToUtf8Bytes))]
[Transient(typeof(IBytesToString), typeof(Utf8BytesToString))]
[Transient(typeof(IOpenerLink), typeof(BrowserOpenerLink))]
[Transient(typeof(IClipboardService), typeof(TopLevelClipboardService))]
public partial class BrowserServiceProvider : IServiceFactory
{
    private readonly IServiceProvider serviceProvider;
        
    public BrowserServiceProvider()
    {
        serviceProvider = this;
    }

    public IServiceFactory ServiceFactoryFactory()
    {
        return DiHelper.ServiceFactory;
    }
        
    public ClientOptions ClientOptionsFactory()
    {
        return new(false);
    }
        
    public IConfiguration ConfigurationFactory()
    {
        using var stream = SpravyUiBrowserMark.GetResourceStream(FileNames.DefaultConfigFileName).ThrowIfNull();

        return new ConfigurationBuilder().AddJsonStream(stream).Build();
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