namespace Spravy.Authentication.Domain.Services;

public class Sha512HashService : IHashService
{
    public byte[] ComputeHash(byte[] input)
    {
        using var sha512Hash = SHA512.Create();
        // Convert the input string to a byte array and compute the hash.
        var data = sha512Hash.ComputeHash(input);

        return data;
    }
}
