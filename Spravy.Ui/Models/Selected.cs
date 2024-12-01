namespace Spravy.Ui.Models;

public partial class Selected<TValue> : NotifyBase where TValue : notnull
{
    [ObservableProperty]
    private bool isSelect;

    public Selected(TValue value)
    {
        Value = value;
    }

    public TValue Value { get; }
}