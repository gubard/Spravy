namespace Spravy.Domain.Errors;

public class VariableInvalidCharsError : Error
{
    public static readonly Guid MainId = new("33A64668-4D6B-4A8C-B431-3ABD36C48E5B");

    protected VariableInvalidCharsError()
        : base(MainId)
    {
        VariableName = string.Empty;
    }

    public VariableInvalidCharsError(ReadOnlyMemory<char> validChars, string variableName)
        : base(MainId)
    {
        ValidChars = validChars;
        VariableName = variableName;
    }

    public string VariableName { get; protected set; }
    public ReadOnlyMemory<char> ValidChars { get; protected set; }

    public override string Message
    {
        get => $"Variable {VariableName} can contains only {ValidChars}";
    }
}
