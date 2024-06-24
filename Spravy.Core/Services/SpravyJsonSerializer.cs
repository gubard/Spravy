using System.Text.Json;
using System.Text.Json.Serialization;

namespace Spravy.Core.Services;

public class SpravyJsonSerializer : ISerializer
{
    private readonly JsonSerializerContext context;

    public SpravyJsonSerializer(JsonSerializerContext context)
    {
        this.context = context;
    }

    public ConfiguredValueTaskAwaitable<Result> SerializeAsync<T>(T obj, Stream stream, CancellationToken ct)
    {
        return SerializeCore(obj, stream, ct).ConfigureAwait(false);
    }

    public async ValueTask<Result> SerializeCore<T>(T obj, Stream stream, CancellationToken ct)
    {
        await JsonSerializer.SerializeAsync(stream, obj, typeof(T) , context, ct);
        
        return Result.Success;
    }

    public ConfiguredValueTaskAwaitable<Result<TObject>> DeserializeAsync<TObject>(Stream stream, CancellationToken ct)
        where TObject : notnull
    {
        throw new NotImplementedException();
    }

    public Result<TObject> Deserialize<TObject>(Stream stream) where TObject : notnull
    {
        throw new NotImplementedException();
    }
}