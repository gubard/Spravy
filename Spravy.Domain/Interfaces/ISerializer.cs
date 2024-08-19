namespace Spravy.Domain.Interfaces;

public interface ISerializer
{
    Result<TObject> Deserialize<TObject>(Stream stream)
        where TObject : notnull;

    ConfiguredValueTaskAwaitable<Result> SerializeAsync<T>(
        T obj,
        Stream stream,
        CancellationToken ct
    )
        where T : notnull;

    ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<byte>>> SerializeAsync<T>(
        T obj,
        CancellationToken ct
    )
        where T : notnull;

    ConfiguredValueTaskAwaitable<Result<TObject>> DeserializeAsync<TObject>(
        Stream stream,
        CancellationToken ct
    )
        where TObject : notnull;

    ConfiguredValueTaskAwaitable<Result<TObject>> DeserializeAsync<TObject>(
        ReadOnlyMemory<byte> content,
        CancellationToken ct
    )
        where TObject : notnull;
}
