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
    >(nameof(IsSelected));

    private ToggleButton? toggleButton;

    static IntegerSelectorItemControl()
    {
        IsSelectedProperty.Changed.AddClassHandler<IntegerSelectorItemControl>(
            (control, _) =>
            {
                if (control.toggleButton is null)
                {
                    return;
                }

                if (control.toggleButton.IsChecked != control.IsSelected)
                {
                    control.toggleButton.IsChecked = control.IsSelected;
                }
            }
        );
    }

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

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        var tb = e.NameScope.Find<ToggleButton>("PART_ToggleButton");
        UpdateToggleButton(tb);
    }

    private void UpdateToggleButton(ToggleButton? newToggleButton)
    {
        if (toggleButton is not null)
        {
            toggleButton.IsCheckedChanged -= OnIsCheckedChanged;
        }

        toggleButton = newToggleButton;

        if (toggleButton is null)
        {
            return;
        }

        toggleButton.IsChecked = IsSelected;
        toggleButton.IsCheckedChanged += OnIsCheckedChanged;
    }

    private void OnIsCheckedChanged(object? sender, RoutedEventArgs args)
    {
        if (sender is not ToggleButton tb)
        {
            return;
        }

        var value = tb.IsChecked.GetValueOrDefault();

        if (IsSelected != value)
        {
            IsSelected = tb.IsChecked.GetValueOrDefault();
        }
    }
}
