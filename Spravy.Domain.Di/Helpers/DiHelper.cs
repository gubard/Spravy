namespace Spravy.Domain.Di.Helpers;

public static class DiHelper
{
    private static IKernel? _kernel;
    
    public static IKernel Kernel
    {
        get => _kernel.ThrowIfNull();
        set => _kernel = value;
    }
}