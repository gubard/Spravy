namespace Spravy.Domain.Errors;

public class VariableNullValueError : Error
{
    public static readonly Guid MainId = new("078427BE-54E1-4502-9B63-A2531876E41B");

    protected VariableNullValueError() : base(MainId)
    {
        VariableName = string.Empty;
    }

    public VariableNullValueError(string variableName) : base(MainId)
    {
        VariableName = variableName;
    }

    public string VariableName { get; protected set; }

    public override string Message => $"Variable {VariableName} can't be null";
}