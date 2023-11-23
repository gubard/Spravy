using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Spravy.Domain.Exceptions;
using Spravy.Domain.Models;

namespace Spravy.Domain.Extensions;

public static class ObjectExtension
{
    public static T[] ToArray<T>(this T obj)
    {
        return new[]
        {
            obj
        };
    }

    public static IEnumerable<T> ToEnumerable<T>(this T obj)
    {
        yield return obj;
    }

    public static void ThrowDisposedException<T>(this T obj) where T : notnull
    {
        throw new ObjectDisposedException<T>(obj);
    }

    public static T ThrowIfIsNot<T>(this object obj)
    {
        if (obj is not T result)
        {
            throw new TypeInvalidCastException(typeof(T), obj.GetType());
        }

        return result;
    }
    
    public static IsSuccessValue<T> ToSuccessValue<T>(this T obj)
    {
        return new IsSuccessValue<T>(obj);
    }

    public static T ThrowIfIsNotCast<T>(this object obj)
    {
        return (T)obj;
    }

    public static T? As<T>(this object value) where T : class
    {
        return value as T;
    }

    public static T To<T>(this object value)
    {
        return (T)value;
    }

    public static T ThrowIfNull<T>(
        this T? obj,
        [CallerArgumentExpression(nameof(obj))]
        string paramName = ""
    )
    {
        if (obj is null)
        {
            throw new ArgumentNullException(paramName);
        }

        return obj;
    }

    public static T ThrowIfNullStruct<T>(
        this T? obj,
        [CallerArgumentExpression(nameof(obj))]
        string paramName = ""
    ) where T : struct
    {
        return obj ?? throw new ArgumentNullException(paramName);
    }

    public static TObj ThrowIfNotEquals<TObj>(
        this TObj obj,
        TObj expected,
        [CallerArgumentExpression(nameof(obj))]
        string name = ""
    )
        where TObj : notnull
    {
        if (obj.Equals(expected))
        {
            return obj;
        }

        throw new NotEqualsException<TObj>(name, obj, expected);
    }

    public static T IfNullUse<T>(this T? obj, T @default)
    {
        if (obj is null)
        {
            return @default;
        }

        return obj;
    }

    public static ConstantExpression ToConstant(this object obj)
    {
        return Expression.Constant(obj);
    }

    public static ConstantExpression ToConstant(this object obj, Type type)
    {
        return Expression.Constant(obj, type);
    }

    public static Ref<T> ToRef<T>(this T value) where T : struct
    {
        return new Ref<T>(value);
    }
    
    public static Task<T> ToTaskResult<T>(this T value)
    {
        return Task.FromResult(value);
    }
}