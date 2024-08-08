using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Spravy.Core.Services;

public class SpravyJsonSerializer : ISerializer
{
    private readonly JsonSerializerContext context;

    public SpravyJsonSerializer(JsonSerializerContext context)
    {
        this.context = context;
    }

    public ConfiguredValueTaskAwaitable<Result> SerializeAsync<T>(
        T obj,
        Stream stream,
        CancellationToken ct
    )
        where T : notnull
    {
        return SerializeCore(obj, stream, ct).ConfigureAwait(false);
    }

    private async ValueTask<Result> SerializeCore<T>(T obj, Stream stream, CancellationToken ct)
        where T : notnull
    {
        await JsonSerializer.SerializeAsync(stream, obj, obj.GetType(), context, ct);

        return Result.Success;
    }

    public ConfiguredValueTaskAwaitable<Result<TObject>> DeserializeAsync<TObject>(
        Stream stream,
        CancellationToken ct
    )
        where TObject : notnull
    {
        return DeserializeCore<TObject>(stream, ct).ConfigureAwait(false);
    }

    private async ValueTask<Result<TObject>> DeserializeCore<TObject>(
        Stream stream,
        CancellationToken ct
    )
        where TObject : notnull
    {
        var result = await JsonSerializer.DeserializeAsync(
            stream,
            (JsonTypeInfo<TObject>)context.GetTypeInfo(typeof(TObject)).ThrowIfNull(),
            ct
        );

        return result.ThrowIfNull().ToResult();
    }

    public Result<TObject> Deserialize<TObject>(Stream stream)
        where TObject : notnull
    {
        var document = JsonDocument.Parse(stream);

        var result = document.Deserialize(
            (JsonTypeInfo<TObject>)context.GetTypeInfo(typeof(TObject)).ThrowIfNull()
        );

        return result.ThrowIfNull().ToResult();
    }
}
