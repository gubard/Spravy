namespace Spravy.Domain.Extensions;

public static class ByteExtension
{
    public static MemoryStream ToMemoryStream(this byte[] bytes)
    {
        return new(bytes);
    }

    public static string ToHex(this byte[] bytes)
    {
        var hex = new StringBuilder(bytes.Length * 2);

        foreach (byte b in bytes)
        {
            hex.AppendFormat("{0:x2}", b);
        }

        return hex.ToString();
    }
}
