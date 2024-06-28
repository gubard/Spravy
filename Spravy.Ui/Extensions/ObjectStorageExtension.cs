namespace Spravy.Ui.Extensions;

public static class ObjectStorageExtension
{
    public static ConfiguredValueTaskAwaitable<Result<TObject>> GetObjectOrDefaultAsync<TObject>(
        this IObjectStorage objectStorage,
        string id,
        CancellationToken ct
    )
        where TObject : IViewModelSetting<TObject>
    {
        return objectStorage
            .IsExistsAsync(id, ct)
            .IfSuccessAsync(
                value =>
                {
                    if (value)
                    {
                        return objectStorage.GetObjectAsync<TObject>(id, ct);
                    }

                    return TObject.Default.ToResult().ToValueTaskResult().ConfigureAwait(false);
                },
                ct
            );
    }
}
