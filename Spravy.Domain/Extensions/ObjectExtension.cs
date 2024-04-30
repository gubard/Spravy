using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Spravy.Domain.Errors;
using Spravy.Domain.Exceptions;
using Spravy.Domain.Helpers;
using Spravy.Domain.Models;

namespace Spravy.Domain.Extensions;

public static class ObjectExtension
{
    public static Result<TValue> ToResult<TValue>(this ReadOnlyMemory<Error> errors)
    {
        return new(errors);
    }

    public static Result<TValue> ToResult<TValue>(this TValue value)
    {
        return new(value);
    }

    public static async Task<T[]> ToArrayAsync<T>(this ConfiguredTaskAwaitable<IEnumerable<T>> task)
    {
        var enumerable = await task;

        return enumerable.ToArray();
    }

    public static async Task<T[]> ToArrayAsync<T>(this Task<IEnumerable<T>> task)
    {
        var enumerable = await task;

        return enumerable.ToArray();
    }

    public static T[] AsArray<T>(this T obj)
    {
        return new[]
        {
            obj,
        };
    }

    public static TObject Case<TObject>(this TObject obj, Action<TObject> action)
    {
        action.Invoke(obj);

        return obj;
    }

    public static TObject Case<TObject>(this TObject obj, Action action)
    {
        action.Invoke();

        return obj;
    }

    public static ReadOnlyMemory<T> Randomize<T>(this ReadOnlyMemory<T> array)
    {
        var rand = DefaultObject<Random>.Default;
        var result = new T[array.Length];

        for (var i = array.Length - 1; i > 0; i--)
        {
            var randomIndex = rand.Next(0, i + 1);
            result[i] = array.Span[randomIndex];
        }

        return result.ToReadOnlyMemory();
    }

    public static uint Max(this ReadOnlyMemory<uint> array)
    {
        if (array.IsEmpty)
        {
            throw new EmptyEnumerableException(nameof(array));
        }

        var result = array.Span[0];
        var span = array.Span;

        for (var i = array.Length - 1; i > 0; i--)
        {
            if (span[i] > result)
            {
                result = span[i];
            }
        }

        return result;
    }

    public static IEnumerable<T> ToEnumerable<T>(this T obj)
    {
        yield return obj;
    }

    public static ReadOnlyMemory<T> ToReadOnlyMemory<T>(this List<T> list)
    {
        return list.ToArray();
    }

    public static ReadOnlyMemory<T> ToReadOnlyMemory<T>(this T[] array)
    {
        return array;
    }

    public static ReadOnlyMemory<T> Combine<T>(this ReadOnlyMemory<T> memory, params ReadOnlyMemory<T>[] memories)
    {
        if (memories.Length == 0)
        {
            return memory;
        }

        var length = memory.Length;

        foreach (var readOnlyMemory in memories)
        {
            length += readOnlyMemory.Length;
        }

        var result = new Memory<T>(new T[length]);
        memory.CopyTo(result);
        var index = memory.Length;

        foreach (var readOnlyMemory in memories)
        {
            readOnlyMemory.Span.CopyTo(result.Span.Slice(index));
            index += readOnlyMemory.Length;
        }

        return result;
    }

    public static ReadOnlyMemory<T> ToReadOnlyMemory<T>(this T obj)
    {
        return new([obj,]);
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
        return new(obj);
    }

    public static T ThrowIfIsNotCast<T>(this object obj)
    {
        return (T)obj;
    }

    public static Result<T> CastObject<T>(this object obj)
    {
        if (obj is T item)
        {
            return item.ToResult();
        }

        return new(new CastError(obj.GetType(), typeof(T)));
    }

    public static T? As<T>(this object value) where T : class
    {
        return value as T;
    }

    public static T To<T>(this object value)
    {
        return (T)value;
    }

    public static T ThrowIfNull<T>(this T? obj, [CallerArgumentExpression(nameof(obj))] string paramName = "")
    {
        if (obj is null)
        {
            throw new ArgumentNullException(paramName);
        }

        return obj;
    }

    public static T ThrowIfNullStruct<T>(this T? obj, [CallerArgumentExpression(nameof(obj))] string paramName = "")
        where T : struct
    {
        return obj ?? throw new ArgumentNullException(paramName);
    }

    public static TObj ThrowIfNotEquals<TObj>(
        this TObj obj,
        TObj expected,
        [CallerArgumentExpression(nameof(obj))] string name = ""
    ) where TObj : notnull
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
        return new(value);
    }

    public static Task<T> ToTaskResult<T>(this T value)
    {
        return Task.FromResult(value);
    }

    public static ValueTask<T> ToValueTaskResult<T>(this T value)
    {
        return ValueTask.FromResult(value);
    }
}