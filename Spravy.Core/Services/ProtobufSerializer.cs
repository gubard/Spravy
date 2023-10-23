using ProtoBuf;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;

namespace Spravy.Core.Services;

public class ProtobufSerializer : ISerializer
{
    public Task SerializeAsync(object obj, Stream stream)
    {
        Serializer.Serialize(stream, obj);

        return Task.CompletedTask;
    }

    public Task<TObject> DeserializeAsync<TObject>(Stream stream)
    {
        return Serializer.Deserialize<TObject>(stream).ToTaskResult();
    }
}