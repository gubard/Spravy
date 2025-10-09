namespace Spravy.Domain.Extensions;

public static class ByteExtension
{
    public static MemoryStream ToMemoryStream(this ReadOnlyMemory<byte> bytes)
    {
        var result = new MemoryStream(bytes.ToArray());
        result.Seek(0, SeekOrigin.Begin);

        return result;
    }
    
    public static MemoryStream ToMemoryStream(this byte[] bytes)
    {
        var result = new MemoryStream(bytes);
        result.Seek(0, SeekOrigin.Begin);

        return result;
    }

    public static string ToHex(this byte[] bytes)
    {
        var hex = new StringBuilder(bytes.Length * 2);

        foreach (var b in bytes)
        {
            hex.AppendFormat("{0:x2}", b);
        }

        return hex.ToString();
    }
}