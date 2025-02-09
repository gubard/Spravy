namespace Spravy.PasswordGenerator.Domain.Services;

public class PasswordGenerator : IPasswordGenerator
{
    private readonly PasswordGeneratorOptions options;
    private readonly IStringToBytes stringToBytes;

    public PasswordGenerator(IStringToBytes stringToBytes, PasswordGeneratorOptions options)
    {
        this.stringToBytes = stringToBytes;
        this.options = options;
    }

    public Result<string> GeneratePassword(string key, GeneratePasswordOptions passwordOptions)
    {
        var bytes = stringToBytes.StringToBytes(key);
        using var sha = SHA512.Create();
        var hash = new ReadOnlySpan<byte>(sha.ComputeHash(bytes));
        var resultBytes = new byte[passwordOptions.Length].AsSpan();
        var availableBytes = SearchValues.Create(Encoding.ASCII.GetBytes(passwordOptions.AvailableCharacters));
        GeneratePassword(resultBytes, hash, availableBytes, sha);
        var tryCount = 0;

        if (passwordOptions.Regex.IsNullOrWhiteSpace())
        {
            return Encoding.ASCII.GetString(resultBytes).ToResult();
        }

        while (!Regex.IsMatch(Encoding.ASCII.GetString(resultBytes), passwordOptions.Regex))
        {
            hash = new(sha.ComputeHash(resultBytes.ToArray()));
            GeneratePassword(resultBytes, hash, availableBytes, sha);
            tryCount++;

            if (tryCount == options.TryCount)
            {
                throw new("Password generation failed");
            }
        }

        return Encoding.ASCII.GetString(resultBytes).ToResult();
    }

    private void GeneratePassword(
        Span<byte> resultBytes,
        ReadOnlySpan<byte> hash,
        SearchValues<byte> availableBytes,
        SHA512 sha
    )
    {
        var hashIndex = 0;
        var currentIndex = 0;

        for (var i = 0; i < resultBytes.Length; i++)
        {
            try
            {
                if (!IsAsciiChar(hash[hashIndex]))
                {
                    i--;

                    continue;
                }

                if (!availableBytes.Contains(hash[hashIndex]))
                {
                    i--;

                    continue;
                }

                resultBytes[i] = hash[hashIndex];
            }
            finally
            {
                hashIndex++;

                if (hash.Length == hashIndex)
                {
                    if (i == currentIndex)
                    {
                        throw new("Password generation failed");
                    }

                    currentIndex = i;
                    hash = new(sha.ComputeHash(resultBytes.ToArray()));
                    hashIndex = 0;
                }
            }
        }
    }

    private bool IsAsciiChar(byte b)
    {
        switch (b)
        {
            case < 32:
            case > 126:
                return false;
            default:
                return true;
        }
    }
}