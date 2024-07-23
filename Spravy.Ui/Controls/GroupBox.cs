using Avalonia.Data;

namespace Spravy.Ui.Controls;

public class GroupBox : ContentControl
{
    public static readonly StyledProperty<object?> HeaderProperty = AvaloniaProperty.Register<
        EnumSelectorControl,
        object?
    >(nameof(Header), defaultBindingMode: BindingMode.TwoWay);

    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }
}
