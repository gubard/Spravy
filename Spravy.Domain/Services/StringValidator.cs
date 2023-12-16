using System.Diagnostics.CodeAnalysis;
using Spravy.Domain.Exceptions;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;

namespace Spravy.Domain.Services;

public class StringValidator : IValidator<SpravyException, string>
{
    private readonly ushort minLength;
    private readonly ushort maxLength;
    private readonly char[] validChars;
    private readonly string parameterName;

    public StringValidator(ushort minLength, ushort maxLength, IEnumerable<char> validChars, string parameterName)
    {
        this.minLength = minLength;
        this.maxLength = maxLength;
        this.parameterName = parameterName;
        this.validChars = validChars.ToArray();
    }

    public bool Validate(string? value, [MaybeNullWhen(true)] out SpravyException exception)
    {
        if (value is null)
        {
            exception = new ParameterNullException(parameterName);

            return false;
        }

        if (value == string.Empty)
        {
            exception = new ParameterEmptyException(parameterName);

            return false;
        }

        if (value.IsNullOrWhiteSpace())
        {
            exception = new ParameterWhiteSpaceException(parameterName);

            return false;
        }

        if (value.Length < minLength)
        {
            exception = new ParameterLengthToSmallException(parameterName, minLength, value.Length);

            return false;
        }

        if (value.Length > maxLength)
        {
            exception = new ParameterLengthToBigException(parameterName, maxLength, value.Length);

            return false;
        }

        var index = value.ToCharArray().AsSpan().IndexOfAnyExcept(validChars);

        if (index > -1)
        {
            exception = new ParameterStringHasValidValuesException(parameterName, validChars, value[index]);

            return false;
        }

        exception = null;

        return true;
    }
}