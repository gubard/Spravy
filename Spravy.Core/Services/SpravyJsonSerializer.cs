namespace Spravy.Core.Services;

public class SpravyJsonSerializer : ISerializer
{
    private readonly JsonSerializerContext context;

    public SpravyJsonSerializer(JsonSerializerContext context)
    {
        this.context = context;
    }

    public Cvtar SerializeAsync<T>(T obj, Stream stream, CancellationToken ct) where T : notnull
    {
        return SerializeCore(obj, stream, ct).ConfigureAwait(false);
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<byte>>> SerializeAsync<T>(T obj, CancellationToken ct)
        where T : notnull
    {
        return SerializeCore(obj, ct).ConfigureAwait(false);
    }

    public ConfiguredValueTaskAwaitable<Result<TObject>> DeserializeAsync<TObject>(Stream stream, CancellationToken ct)
        where TObject : notnull
    {
        return DeserializeCore<TObject>(stream, ct).ConfigureAwait(false);
    }

    public ConfiguredValueTaskAwaitable<Result<TObject>> DeserializeAsync<TObject>(
        ReadOnlyMemory<byte> content,
        CancellationToken ct
    ) where TObject : notnull
    {
        return DeserializeCore<TObject>(content, ct).ConfigureAwait(false);
    }

    public Result<TObject> Deserialize<TObject>(Stream stream) where TObject : notnull
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

    private async ValueTask<Result<ReadOnlyMemory<byte>>> SerializeCore<T>(T obj, CancellationToken ct)
        where T : notnull
    {
        await using var stream = new MemoryStream();

        await JsonSerializer.SerializeAsync(
            stream,
            obj,
            obj.GetType(),
            context,
            ct
        );

        stream.Position = 0;

        return new(stream.ToArray());
    }

    private async ValueTask<Result> SerializeCore<T>(T obj, Stream stream, CancellationToken ct) where T : notnull
    {
        await JsonSerializer.SerializeAsync(
            stream,
            obj,
            obj.GetType(),
            context,
            ct
        );

        return Result.Success;
    }

    private async ValueTask<Result<TObject>> DeserializeCore<TObject>(
        ReadOnlyMemory<byte> content,
        CancellationToken ct
    ) where TObject : notnull
    {
        var typeInfo = context.GetTypeInfo(typeof(TObject));

        if (typeInfo is null)
        {
            return new(new NotFoundTypeError(typeof(TObject)));
        }

        await using var stream = new MemoryStream(content.ToArray());
        stream.Position = 0;
        var result = await JsonSerializer.DeserializeAsync(stream, (JsonTypeInfo<TObject>)typeInfo, ct);

        return result.ThrowIfNull().ToResult();
    }

    private async ValueTask<Result<TObject>> DeserializeCore<TObject>(Stream stream, CancellationToken ct)
        where TObject : notnull
    {
        var typeInfo = context.GetTypeInfo(typeof(TObject));

        if (typeInfo is null)
        {
            return new(new NotFoundTypeError(typeof(TObject)));
        }

        var result = await JsonSerializer.DeserializeAsync(stream, (JsonTypeInfo<TObject>)typeInfo, ct);

        return result.ThrowIfNull().ToResult();
    }
}