using System.Text.Json;
using System.Text.Json.Serialization;

namespace Spravy.Core.Services;

public class SpravyJsonSerializer : ISerializer
{
    private readonly JsonSerializerOptions options;

    public SpravyJsonSerializer(JsonSerializerContext context)
    {
        options = new()
        {
            TypeInfoResolver = context,
        };
    }

    public ConfiguredValueTaskAwaitable<Result> SerializeAsync<T>(T obj, Stream stream, CancellationToken ct)
    {
        return SerializeCore(obj, stream, ct).ConfigureAwait(false);
    }

    private async ValueTask<Result> SerializeCore<T>(T obj, Stream stream, CancellationToken ct)
    {
        await JsonSerializer.SerializeAsync(stream, obj, options, ct);

        return Result.Success;
    }

    public ConfiguredValueTaskAwaitable<Result<TObject>> DeserializeAsync<TObject>(Stream stream, CancellationToken ct)
        where TObject : notnull
    {
        return DeserializeCore<TObject>(stream, ct).ConfigureAwait(false);
    }

    private async ValueTask<Result<TObject>> DeserializeCore<TObject>(Stream stream, CancellationToken ct)
        where TObject : notnull
    {
        var result = await JsonSerializer.DeserializeAsync<TObject>(stream, options, ct);

        return result.ThrowIfNull().ToResult();
    }

    public Result<TObject> Deserialize<TObject>(Stream stream) where TObject : notnull
    {
        var result = JsonSerializer.Deserialize<TObject>(stream, options);

        return result.ThrowIfNull().ToResult();
    }
}