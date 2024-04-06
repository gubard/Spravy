using Grpc.Core;
using Spravy.Domain.Exceptions;
using Spravy.Domain.Helpers;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.Service.Extensions;

public static class ResultExtension
{
    public static async Task<TReturn> HandleAsync<TReturn>(
        this ValueTask<Result> task,
        ISerializer serializer
    ) where TReturn : class, new()
    {
        var result = await task;

        if (result.IsHasError)
        {
            throw await result.ToRpcExceptionAsync(serializer);
        }

        return DefaultObject<TReturn>.Default;
    }

    public static async Task<TReturn> HandleAsync<TReturn>(
        this ValueTask<Result> task,
        ISerializer serializer,
        Func<TReturn> func
    )
    {
        var result = await task;

        if (result.IsHasError)
        {
            throw await result.ToRpcExceptionAsync(serializer);
        }

        return func.Invoke();
    }

    public static async Task<TReturn> HandleAsync<TValue, TReturn>(
        this ValueTask<Result<TValue>> task,
        ISerializer serializer,
        Func<TValue, TReturn> func
    )
    {
        var result = await task;

        if (result.IsHasError)
        {
            throw await result.ToRpcExceptionAsync(serializer);
        }

        return func.Invoke(result.Value);
    }

    public static async Task<TValue> HandleAsync<TValue>(
        this ValueTask<Result<TValue>> task,
        ISerializer serializer
    )
    {
        var result = await task;

        if (result.IsHasError)
        {
            throw await result.ToRpcExceptionAsync(serializer);
        }

        return result.Value;
    }

    public static async ValueTask<Metadata> GetMetadataAsync<TValue>(this Result<TValue> result, ISerializer serializer)
    {
        var metadata = new Metadata();

        foreach (var validationResult in result.Errors.ToArray())
        {
            await using var stream = new MemoryStream();
            await serializer.Serialize(validationResult, stream);
            metadata.Add($"{validationResult.Id}-bin", stream.ToArray());
        }

        return metadata;
    }

    public static async ValueTask<Metadata> GetMetadataAsync(this Result result, ISerializer serializer)
    {
        var metadata = new Metadata();

        foreach (var validationResult in result.Errors.ToArray())
        {
            await using var stream = new MemoryStream();
            await serializer.Serialize(validationResult, stream);
            metadata.Add($"{validationResult.Id}-bin", stream.ToArray());
        }

        return metadata;
    }

    public static async ValueTask<RpcException> ToRpcExceptionAsync(this Result result, ISerializer serializer)
    {
        if (!result.IsHasError)
        {
            throw new EmptyEnumerableException(nameof(result.Errors));
        }

        return new RpcException(
            new Status(StatusCode.InvalidArgument, result.GetTitle()),
            await result.GetMetadataAsync(serializer)
        );
    }

    public static async ValueTask<RpcException> ToRpcExceptionAsync<TValue>(
        this Result<TValue> result,
        ISerializer serializer
    )
    {
        if (!result.IsHasError)
        {
            throw new EmptyEnumerableException(nameof(result.Errors));
        }

        return new RpcException(
            new Status(StatusCode.InvalidArgument, result.GetTitle()),
            await result.GetMetadataAsync(serializer)
        );
    }
}