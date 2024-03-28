using Grpc.Core;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.Client.Extensions;

public static class RpcExceptionExtension
{
    public static async Task<Error> ToErrorAsync(this RpcException exception, ISerializer serializer)
    {
        var validationResults = new List<ValidationResult>();

        foreach (var trailer in exception.Trailers)
        {
            if (!Guid.TryParse(trailer.Key, out var guid))
            {
                continue;
            }

            await using var stream = new MemoryStream();
            await stream.WriteAsync(trailer.ValueBytes);
            var validationResult = await serializer.DeserializeAsync<ValidationResult>(stream);
            validationResults.Add(validationResult);
        }

        if (validationResults.Any())
        {
            return new Error(validationResults.ToArray());
        }

        return new Error();
    }
}