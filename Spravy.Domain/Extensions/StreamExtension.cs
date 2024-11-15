namespace Spravy.Domain.Extensions;

public static class StreamExtension
{
    public static Span<byte> ToByteArray(this Stream stream)
    {
        Span<byte> buffer = new byte[stream.Length];
        var length = stream.Read(buffer);

        return buffer.Slice(0, length);
    }

    public static async Task<byte[]> ToByteArrayAsync(this Stream stream)
    {
        var buffer = new byte[stream.Length];
        await stream.ReadExactlyAsync(buffer);

        return buffer;
    }
}
