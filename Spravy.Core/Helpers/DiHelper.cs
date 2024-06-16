namespace Spravy.Core.Helpers;

public static class DiHelper
{
    private static IServiceFactory? _kernel;
    
    public static IServiceFactory ServiceFactory
    {
        get => _kernel.ThrowIfNull();
        set => _kernel = value;
    }
}