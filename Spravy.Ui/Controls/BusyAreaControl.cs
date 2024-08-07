namespace Spravy.Ui.Controls;

public class BusyAreaControl : ContentControl
{
    public static readonly StyledProperty<object?> BusyAreaProperty = AvaloniaProperty.Register<
        BusyAreaControl,
        object?
    >(nameof(BusyArea));

    public static readonly StyledProperty<bool> IsBusyProperty = AvaloniaProperty.Register<
        BusyAreaControl,
        bool
    >(nameof(IsBusy));

    static BusyAreaControl()
    {
        IsBusyProperty.Changed.AddClassHandler<BusyAreaControl>(
            (control, _) => control.PseudoClasses.Set(":is-busy", control.IsBusy)
        );
    }

    public bool IsBusy
    {
        get => GetValue(IsBusyProperty);
        set => SetValue(IsBusyProperty, value);
    }

    public object? BusyArea
    {
        get => GetValue(BusyAreaProperty);
        set => SetValue(BusyAreaProperty, value);
    }
}
