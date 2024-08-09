namespace Spravy.Domain.Extensions;

public static class ObjectExtensionV2
{
    public static OptionStruct<TValue> ToOption<TValue>(this TValue value)
        where TValue : struct
    {
        return new(value);
    }
}

public static class ObjectExtension
{
    public static Option<TValue> ToOption<TValue>(this TValue? value)
        where TValue : class
    {
        if (value is null)
        {
            return new();
        }

        return new(value);
    }

    public static OptionStruct<TValue> ToOption<TValue>(this TValue? value)
        where TValue : struct
    {
        if (value is null)
        {
            return new();
        }

        return new(value.Value);
    }

    public static Result<TResult> IfIs<TResult>(this object obj)
        where TResult : notnull
    {
        if (obj is TResult result)
        {
            return result.ToResult();
        }

        return new(new CastError(obj.GetType(), typeof(TResult)));
    }

    public static Result<TValue> ToResult<TValue>(this ReadOnlyMemory<Error> errors)
        where TValue : notnull
    {
        return new(errors);
    }

    public static Result<TValue> ToResult<TValue>(this TValue value)
        where TValue : notnull
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
        return new[] { obj, };
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

    public static ReadOnlyMemory<T> Combine<T>(
        this ReadOnlyMemory<T> memory,
        ReadOnlyMemory<T> value
    )
    {
        if (value.Length == 0)
        {
            return memory;
        }

        var result = new Memory<T>(new T[memory.Length + value.Length]);
        memory.CopyTo(result);
        value.CopyTo(result.Slice(memory.Length));

        return result;
    }

    public static ReadOnlyMemory<T> Combine<T>(
        this ReadOnlyMemory<T> memory,
        params ReadOnlyMemory<T>[] memories
    )
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

        for (var i = 0; i < memories.Length; i++)
        {
            if (memories[i].IsEmpty)
            {
                continue;
            }

            memories[i].CopyTo(result.Slice(index));
            index += memories[i].Length;
        }

        return result;
    }

    public static ReadOnlyMemory<T> ToReadOnlyMemory<T>(this T obj)
    {
        return new([obj,]);
    }

    public static ReadOnlyMemory<T> ToReadOnlyMemory<T>(this Memory<T> memory)
    {
        return memory;
    }

    public static Result<T> IfNotNull<T>(this T? obj, string propertyName)
        where T : notnull
    {
        if (obj is null)
        {
            return new(new PropertyNullValueError(propertyName));
        }

        return new(obj);
    }

    public static Result<T> IfNotNullStruct<T>(this T? obj, string propertyName)
        where T : struct
    {
        if (obj is null)
        {
            return new(new PropertyNullValueError(propertyName));
        }

        return new(obj.Value);
    }

    public static void ThrowDisposedException<T>(this T obj)
        where T : notnull
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
        where T : notnull
    {
        if (obj is T item)
        {
            return item.ToResult();
        }

        return new(new CastError(obj.GetType(), typeof(T)));
    }

    public static T? As<T>(this object value)
        where T : class
    {
        return value as T;
    }

    public static T To<T>(this object value)
    {
        return (T)value;
    }

    public static T ThrowIfNull<T>(
        this T? obj,
        [CallerArgumentExpression(nameof(obj))] string paramName = ""
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
        [CallerArgumentExpression(nameof(obj))] string paramName = ""
    )
        where T : struct
    {
        return obj ?? throw new ArgumentNullException(paramName);
    }

    public static TObj ThrowIfNotEquals<TObj>(
        this TObj obj,
        TObj expected,
        [CallerArgumentExpression(nameof(obj))] string name = ""
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

    public static Ref<T> ToRef<T>(this T value)
        where T : struct
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
