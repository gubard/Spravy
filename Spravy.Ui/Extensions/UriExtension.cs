using Avalonia.Platform;

namespace Spravy.Ui.Extensions;

public static class UriExtension
{
    public static ReadOnlyMemory<byte> GetAssetBytes(this Uri uri)
    {
        using var stream = AssetLoader.Open(uri);
        var span = stream.ToByteArray();
        return span.ToArray();
    }
}
