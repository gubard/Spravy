namespace Spravy.Domain.Extensions;

public static class CharExtension
{
    public static bool AreEquals(this ReadOnlySpan<char> x, ReadOnlySpan<char> y)
    {
        if (x.IsEmpty && y.IsEmpty)
        {
            return true;
        }

        if (x.Length != y.Length)
        {
            return false;
        }

        for (var i = 0; i < x.Length; i++)
        {
            if (x[i] != y[i])
            {
                return false;
            }
        }

        return true;
    }
}