using Grpc.Core;
using Spravy.Domain.Exceptions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.Service.Extensions;

public static class ErrorExtension
{
    public static async Task<Metadata> GetMetadataAsync(this Error error, ISerializer serializer)
    {
        var metadata = new Metadata();

        foreach (var result in error.ValidationResults.ToArray())
        {
            await using var stream = new MemoryStream();
            serializer.Serialize(result, stream);
            metadata.Add($"{result.Id}-bin", stream.ToArray());
        }

        return metadata;
    }

    public static async Task<RpcException> ToRpcExceptionAsync(this Error error, ISerializer serializer)
    {
        return new RpcException(
            new Status(StatusCode.InvalidArgument, error.GetTitle()),
            await error.GetMetadataAsync(serializer)
        );
    }
}