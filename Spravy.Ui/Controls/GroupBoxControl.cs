namespace Spravy.Ui.Controls;

public class GroupBoxControl : ContentControl
{
    public static readonly StyledProperty<object?> HeaderProperty = AvaloniaProperty.Register<
        GroupBoxControl,
        object?
    >(nameof(Header), defaultBindingMode: BindingMode.TwoWay);

    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }
}
