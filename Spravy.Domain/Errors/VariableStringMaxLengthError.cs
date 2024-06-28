namespace Spravy.Domain.Errors;

public class VariableStringMaxLengthError : Error
{
    public static readonly Guid MainId = new("B554DD15-82E1-4B54-AEB4-88CFF95CCCEA");

    protected VariableStringMaxLengthError()
        : base(MainId)
    {
        VariableName = string.Empty;
    }

    public VariableStringMaxLengthError(ushort maxLength, string variableName, uint variableLength)
        : base(MainId)
    {
        MaxLength = maxLength;
        VariableName = variableName;
        VariableLength = variableLength;
    }

    public string VariableName { get; protected set; }
    public ushort MaxLength { get; protected set; }
    public uint VariableLength { get; protected set; }

    public override string Message
    {
        get =>
            $"Variable {VariableName} length can't be more then {MaxLength}, but current length {VariableLength}";
    }
}
