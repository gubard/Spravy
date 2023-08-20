using System.Reflection;
using Spravy.Domain.Models;

namespace Spravy.Domain.Interfaces;

public interface IInvoker
{
    object? Invoke(Delegate del, DictionarySpan<TypeInformation, object> arguments);
    object? Invoke(object? obj, MethodInfo method, DictionarySpan<TypeInformation, object> arguments);
}