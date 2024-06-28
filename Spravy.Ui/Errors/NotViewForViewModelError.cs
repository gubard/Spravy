namespace Spravy.Ui.Errors;

public class NotViewForViewModelError : Error
{
    public static readonly Guid MainId = new("581F4157-B1D1-444C-80B9-9A093BF98D17");

    protected NotViewForViewModelError()
        : base(MainId)
    {
        ViewModelType = string.Empty;
    }

    public NotViewForViewModelError(string viewModelType)
        : base(MainId)
    {
        ViewModelType = viewModelType;
    }

    public string ViewModelType { get; }

    public override string Message
    {
        get => $"Not found view for {ViewModelType}";
    }
}
