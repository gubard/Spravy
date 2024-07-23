using Avalonia.Data;
using Avalonia.Metadata;

namespace Spravy.Ui.Controls;

public class BusyArea : ContentControl
{
    public static readonly StyledProperty<bool> IsBusyProperty = AvaloniaProperty.Register<
        EnumSelectorControl,
        bool
    >(nameof(IsBusy), defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<string?> BusyTextProperty = AvaloniaProperty.Register<
        EnumSelectorControl,
        string?
    >(nameof(BusyText), defaultBindingMode: BindingMode.TwoWay);

    public string? BusyText
    {
        get => GetValue(BusyTextProperty);
        set => SetValue(BusyTextProperty, value);
    }

    public bool IsBusy
    {
        get => GetValue(IsBusyProperty);
        set => SetValue(IsBusyProperty, value);
    }
}
