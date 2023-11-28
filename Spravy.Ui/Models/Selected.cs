using ReactiveUI;

namespace Spravy.Ui.Models;

public class Selected<TValue> : NotifyBase
{
    private bool isSelect;

    public Selected(TValue value)
    {
        Value = value;
    }

    public bool IsSelect
    {
        get => isSelect;
        set => this.RaiseAndSetIfChanged(ref isSelect, value);
    }

    public TValue Value { get; }
}