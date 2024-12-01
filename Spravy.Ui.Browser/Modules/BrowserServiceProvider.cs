namespace Spravy.Ui.Browser.Modules;

[ServiceProvider]
[Import(typeof(IUiModule))]
[Singleton(typeof(IFactory<ISingleViewTopLevelControl>), typeof(BrowserSingleViewTopLevelControlFactory))]
[Singleton(typeof(IConfigurationLoader), typeof(EmbeddedConfigurationLoader))]
[Singleton(typeof(IConfiguration), Factory = nameof(ConfigurationFactory))]
[Singleton(typeof(ClientOptions), Factory = nameof(ClientOptionsFactory))]
[Singleton(typeof(IServiceFactory), Factory = nameof(ServiceFactoryFactory))]
[Transient(typeof(IStringToBytes), typeof(StringToUtf8Bytes))]
[Transient(typeof(IBytesToString), typeof(Utf8BytesToString))]
[Transient(typeof(IOpenerLink), typeof(BrowserOpenerLink))]
[Transient(typeof(ISoundPlayer), typeof(SoundPlayer))]
[Transient(typeof(IClipboardService), typeof(AvaloniaClipboardService))]
[Transient(typeof(IObjectStorage), typeof(LocalStorageObjectStorage))]
public partial class BrowserServiceProvider : IServiceFactory
{
    public T CreateService<T>() where T : notnull
    {
        return GetService<T>();
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
}