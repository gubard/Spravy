namespace Spravy.Service.Extensions;

public static class ResultExtension
{
    public static async Task<TReturn> HandleAsync<TReturn>(
        this ConfiguredValueTaskAwaitable<Result> task,
        ISerializer serializer,
        CancellationToken ct
    )
        where TReturn : class, new()
    {
        var result = await task;

        if (result.IsHasError)
        {
            throw await result.ToRpcExceptionAsync(serializer, ct);
        }

        return DefaultObject<TReturn>.Default;
    }

    public static async Task<TReturn> HandleAsync<TReturn>(
        this ConfiguredValueTaskAwaitable<Result> task,
        ISerializer serializer,
        Func<TReturn> func,
        CancellationToken ct
    )
    {
        var result = await task;

        if (result.IsHasError)
        {
            throw await result.ToRpcExceptionAsync(serializer, ct);
        }

        return func.Invoke();
    }

    public static async Task<TReturn> HandleAsync<TValue, TReturn>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        ISerializer serializer,
        Func<TValue, TReturn> func,
        CancellationToken ct
    )
        where TValue : notnull
    {
        var result = await task;

        if (result.IsHasError)
        {
            throw await result.ToRpcExceptionAsync(serializer, ct);
        }

        return func.Invoke(result.Value);
    }

    public static async Task<TValue> HandleAsync<TValue>(
        this ConfiguredValueTaskAwaitable<Result<TValue>> task,
        ISerializer serializer,
        CancellationToken ct
    )
        where TValue : notnull
    {
        var result = await task;

        if (result.IsHasError)
        {
            throw await result.ToRpcExceptionAsync(serializer, ct);
        }

        return result.Value;
    }

    public static async ValueTask<Metadata> GetMetadataAsync<TValue>(
        this Result<TValue> result,
        ISerializer serializer,
        CancellationToken ct
    )
        where TValue : notnull
    {
        var metadata = new Metadata();

        foreach (var validationResult in result.Errors.ToArray())
        {
            await using var stream = new MemoryStream();
            await serializer.SerializeAsync(validationResult, stream, ct);
            metadata.Add($"{validationResult.Id}-bin", stream.ToArray());
        }

        return metadata;
    }

    public static async ValueTask<Metadata> GetMetadataAsync(
        this Result result,
        ISerializer serializer,
        CancellationToken ct
    )
    {
        var metadata = new Metadata();

        foreach (var validationResult in result.Errors.ToArray())
        {
            await using var stream = new MemoryStream();
            await serializer.SerializeAsync(validationResult, stream, ct);
            metadata.Add($"{validationResult.Id}-bin", stream.ToArray());
        }

        return metadata;
    }

    public static async ValueTask<RpcException> ToRpcExceptionAsync(
        this Result result,
        ISerializer serializer,
        CancellationToken ct
    )
    {
        if (!result.IsHasError)
        {
            throw new EmptyEnumerableException(nameof(result.Errors));
        }

        return new(
            new(StatusCode.InvalidArgument, result.GetTitle()),
            await result.GetMetadataAsync(serializer, ct)
        );
    }

    public static async ValueTask<RpcException> ToRpcExceptionAsync<TValue>(
        this Result<TValue> result,
        ISerializer serializer,
        CancellationToken ct
    )
        where TValue : notnull
    {
        if (!result.IsHasError)
        {
            throw new EmptyEnumerableException(nameof(result.Errors));
        }

        return new(
            new(StatusCode.InvalidArgument, result.GetTitle()),
            await result.GetMetadataAsync(serializer, ct)
        );
    }
}
