namespace Spravy.Ui.Errors;

public class AllItemSelectedError : Error
{
    public static readonly Guid MainId = new("57F4876B-08D4-4D02-91D5-B2A0591D4766");

    public AllItemSelectedError()
        : base(MainId) { }

    public override string Message
    {
        get => "All items selected.";
    }
}
