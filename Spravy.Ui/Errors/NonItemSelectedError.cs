namespace Spravy.Ui.Errors;

public class NonItemSelectedError : Error
{
    public static readonly Guid MainId = new("10B7FEA9-2630-4261-99EC-6934631CD172");

    public NonItemSelectedError() : base(MainId)
    {
    }

    public override string Message => "Non item selected.";
}