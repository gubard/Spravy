namespace Spravy.Domain.Extensions;

public static class StreamExtension
{
    public static byte[] ToByteArray(this Stream stream)
    {
        var buffer = new byte[stream.Length];
        stream.Read(buffer);

        return buffer;
    }

    public static async Task<byte[]> ToByteArrayAsync(this Stream stream)
    {
        var buffer = new byte[stream.Length];
        await stream.ReadAsync(buffer);

        return buffer;
    }
}