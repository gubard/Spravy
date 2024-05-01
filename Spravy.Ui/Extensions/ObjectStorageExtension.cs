namespace Spravy.Ui.Extensions;

public static class ObjectStorageExtension
{
    public static ConfiguredValueTaskAwaitable<Result<TObject>> GetObjectOrDefaultAsync<TObject>(
        this IObjectStorage objectStorage,
        string id,
        CancellationToken cancellationToken
    ) where TObject : IViewModelSetting<TObject>
    {
        return objectStorage.IsExistsAsync(id)
           .IfSuccessAsync(value =>
            {
                if (value)
                {
                    return objectStorage.GetObjectAsync<TObject>(id);
                }

                return TObject.Default.ToResult().ToValueTaskResult().ConfigureAwait(false);
            }, cancellationToken);
    }
}