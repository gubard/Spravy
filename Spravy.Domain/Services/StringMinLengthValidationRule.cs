namespace Spravy.Domain.Services;

public class StringMinLengthValidationRule : IValidationRule<string>
{
    private readonly ushort minLength;

    public StringMinLengthValidationRule(ushort minLength)
    {
        this.minLength = minLength;
    }

    public Cvtar ValidateAsync(string? value, string sourceName)
    {
        if (value is null)
        {
            return Result.AwaitableSuccess;
        }

        if (value.Length < minLength)
        {
            return new Result(new VariableStringMinLengthError(minLength, sourceName, (uint)value.Length))
               .ToValueTaskResult()
               .ConfigureAwait(false);
        }

        return Result.AwaitableSuccess;
    }
}