namespace Spravy.Domain.Errors;

public class VariableStringMinLengthError : Error
{
    public static readonly Guid MainId = new("899C375F-FA4E-4A94-8034-62FCA6E91D93");

    protected VariableStringMinLengthError()
        : base(MainId)
    {
        VariableName = string.Empty;
    }

    public VariableStringMinLengthError(ushort minLength, string variableName, uint variableLength)
        : base(MainId)
    {
        MinLength = minLength;
        VariableName = variableName;
        VariableLength = variableLength;
    }

    public string VariableName { get; protected set; }
    public ushort MinLength { get; protected set; }
    public uint VariableLength { get; protected set; }

    public override string Message
    {
        get =>
            $"Variable {VariableName} length can't be less then {MinLength}, but current length {VariableLength}";
    }
}
