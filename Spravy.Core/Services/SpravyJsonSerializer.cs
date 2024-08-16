using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Spravy.Domain.Errors;

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
        var typeInfo = context.GetTypeInfo(typeof(TObject));

        if (typeInfo is null)
        {
            return new(new NotFoundTypeError(typeof(TObject)));
        }

        var result = await JsonSerializer.DeserializeAsync(
            stream,
            (JsonTypeInfo<TObject>)typeInfo,
            ct
        );

        return result.ThrowIfNull().ToResult();
    }

    public Result<TObject> Deserialize<TObject>(Stream stream)
        where TObject : notnull
    {
        var document = JsonDocument.Parse(stream);
        var typeInfo = context.GetTypeInfo(typeof(TObject));

        if (typeInfo is null)
        {
            return new(new NotFoundTypeError(typeof(TObject)));
        }

        var result = document.Deserialize((JsonTypeInfo<TObject>)typeInfo);

        return result.ThrowIfNull().ToResult();
    }
}
