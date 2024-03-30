using Grpc.Core;
using Spravy.Domain.Exceptions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.Service.Extensions;

public static class ResultExtension
{
    public static async Task<Metadata> GetMetadataAsync(this Result result, ISerializer serializer)
    {
        var metadata = new Metadata();

        foreach (var validationResult in result.ValidationResults.ToArray())
        {
            await using var stream = new MemoryStream();
            serializer.Serialize(validationResult, stream);
            metadata.Add($"{validationResult.Id}-bin", stream.ToArray());
        }

        return metadata;
    }

    public static async Task<RpcException> ToRpcExceptionAsync(this Result result, ISerializer serializer)
    {
        return new RpcException(
            new Status(StatusCode.InvalidArgument, result.GetTitle()),
            await result.GetMetadataAsync(serializer)
        );
    }
}