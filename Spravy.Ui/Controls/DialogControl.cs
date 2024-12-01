namespace Spravy.Ui.Controls;

public class DialogControl : ContentControl
{
    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<DialogControl, bool>(nameof(IsOpen), defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<object?> DialogProperty =
        AvaloniaProperty.Register<DialogControl, object?>(nameof(Dialog), defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<double> WidthDialogProperty =
        AvaloniaProperty.Register<DialogControl, double>(nameof(WidthDialog), double.NaN);

    public static readonly StyledProperty<double> HeightDialogProperty =
        AvaloniaProperty.Register<DialogControl, double>(nameof(HeightDialog), double.NaN);

    public static readonly StyledProperty<double> MaxWidthDialogProperty =
        AvaloniaProperty.Register<DialogControl, double>(nameof(MaxWidthDialog), double.PositiveInfinity);

    public static readonly StyledProperty<double> MaxHeightDialogProperty =
        AvaloniaProperty.Register<DialogControl, double>(nameof(MaxHeightDialog), double.PositiveInfinity);

    static DialogControl()
    {
        IsOpenProperty.Changed.AddClassHandler<DialogControl>(
            (control, _) => control.PseudoClasses.Set(":is-open", control.IsOpen)
        );
    }

    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    public object? Dialog
    {
        get => GetValue(DialogProperty);
        set => SetValue(DialogProperty, value);
    }

    public double WidthDialog
    {
        get => GetValue(WidthDialogProperty);
        set => SetValue(WidthDialogProperty, value);
    }

    public double HeightDialog
    {
        get => GetValue(MaxWidthDialogProperty);
        set => SetValue(MaxWidthDialogProperty, value);
    }

    public double MaxWidthDialog
    {
        get => GetValue(WidthDialogProperty);
        set => SetValue(WidthDialogProperty, value);
    }

    public double MaxHeightDialog
    {
        get => GetValue(MaxHeightDialogProperty);
        set => SetValue(MaxHeightDialogProperty, value);
    }
}