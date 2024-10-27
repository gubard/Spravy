namespace Spravy.Authentication.Domain.Services;

public class Md5HashService : IHashService
{
    public byte[] ComputeHash(byte[] input)
    {
        using var md5 = MD5.Create();

        return md5.ComputeHash(input);
    }
}