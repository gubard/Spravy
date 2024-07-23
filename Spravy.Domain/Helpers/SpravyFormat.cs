namespace Spravy.Domain.Helpers;

public static class SpravyFormat
{
    public static Result<string> Format(string input, IParameters parameters)
    {
        return Format(input.AsSpan(), parameters);
    }

    public static Result<string> Format(ReadOnlySpan<char> input, IParameters parameters)
    {
        var currentSlice = input;
        var result = new StringBuilder();

        while (true)
        {
            var start = currentSlice.IndexOf('{');

            if (start == -1)
            {
                result.Append(currentSlice);

                return result.ToString().ToResult();
            }

            var end = currentSlice.IndexOf('}');

            if (end == -1)
            {
                result.Append(currentSlice);

                return result.ToString().ToResult();
            }

            result.Append(currentSlice.Slice(0, start));
            var parameterName = currentSlice.Slice(start + 1, end - start - 1);
            currentSlice = currentSlice.Slice(end + 1);
            var parameterValue = parameters.GetParameter(parameterName);

            if (!parameterValue.TryGetValue(out var value))
            {
                return parameterValue;
            }

            result.Append(value);
        }
    }
}
