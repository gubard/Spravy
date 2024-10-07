namespace Spravy.Ui.Controls;

public class IconSelectorControl : TemplatedControl
{
    public static readonly StyledProperty<string?> SelectedIconProperty = AvaloniaProperty.Register<
        IconSelectorControl,
        string?
    >(nameof(SelectedIcon), defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<IEnumerable<string>?> ItemsSourceProperty =
        AvaloniaProperty.Register<IconSelectorControl, IEnumerable<string>?>(nameof(ItemsSource));

    public string? SelectedIcon
    {
        get => GetValue(SelectedIconProperty);
        set => SetValue(SelectedIconProperty, value);
    }

    public IEnumerable<string>? ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }
}
