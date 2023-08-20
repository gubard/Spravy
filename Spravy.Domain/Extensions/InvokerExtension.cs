using System.Reflection;
using Spravy.Domain.Interfaces;

namespace Spravy.Domain.Extensions;

public static class InvokerExtension
{
    public static object? Invoke<TInvoker>(this TInvoker invoker, Delegate del) where TInvoker : IInvoker
    {
        return invoker.Invoke(del, new ());
    }
    
    public static object? Invoke<TInvoker>(this TInvoker invoker, object obj, MethodInfo methodInfo) where TInvoker : IInvoker
    {
        return invoker.Invoke(obj, methodInfo, new ());
    }
}