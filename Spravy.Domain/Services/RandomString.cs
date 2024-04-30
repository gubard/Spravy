using System.Security.Cryptography;
using Spravy.Domain.Interfaces;

namespace Spravy.Domain.Services;

public class RandomString : IRandom<string>
{
    private readonly byte length;
    private readonly string values;

    public RandomString(string values, byte length)
    {
        this.values = values;
        this.length = length;
    }

    public string GetRandom()
    {
        var span = new Span<char>(new char[length]);

        for (var i = 0; i < length; i++)
        {
            var index = RandomNumberGenerator.GetInt32(0, values.Length);
            span[i] = values[index];
        }

        return span.ToString();
    }
}