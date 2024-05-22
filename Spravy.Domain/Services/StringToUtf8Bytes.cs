namespace Spravy.Domain.Services;

public class StringToUtf8Bytes : IStringToBytes
{
    public byte[] StringToBytes(string input)
    {
        return Encoding.UTF8.GetBytes(input);
    }
}