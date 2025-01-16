using Avalonia.Platform.Storage;

namespace Spravy.Ui.Extensions;

public static class StorageProviderExtension
{
    public static async ValueTask<Result<IReadOnlyList<IStorageFile>>> SpravyOpenFilePickerAsync(
        this IStorageProvider storageProvider,
        FilePickerOpenOptions options
    )
    {
        var files = await storageProvider.OpenFilePickerAsync(options);

        return files.ToResult();
    }
}