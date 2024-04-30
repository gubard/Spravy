namespace Spravy.Domain.Extensions;

public static class ByteExtension
{
    public static MemoryStream ToMemoryStream(this byte[] bytes)
    {
        return new(bytes);
    }
}