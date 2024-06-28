namespace Spravy.Domain.Services;

public class Utf8BytesToString : IBytesToString
{
    public string BytesToString(byte[] input)
    {
        return Encoding.UTF8.GetString(input);
    }
}
