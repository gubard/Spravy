using Avalonia.Data;

namespace Spravy.Ui.Controls;

public class IntegerSelectorItemControl : TemplatedControl
{
    public static readonly StyledProperty<int> ValueProperty = AvaloniaProperty.Register<
        IntegerSelectorItemControl,
        int
    >(nameof(Value));

    public static readonly StyledProperty<bool> IsSelectedProperty = AvaloniaProperty.Register<
        IntegerSelectorItemControl,
        bool
    >(nameof(IsSelected), defaultBindingMode: BindingMode.TwoWay);

    public int Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public bool IsSelected
    {
        get => GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }
}
