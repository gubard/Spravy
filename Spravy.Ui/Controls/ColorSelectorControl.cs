namespace Spravy.Ui.Controls;

public class ColorSelectorControl : TemplatedControl
{
    public static readonly StyledProperty<Color?> SelectedColorProperty =
        AvaloniaProperty.Register<ColorSelectorControl, Color?>(
            nameof(SelectedColor),
            defaultBindingMode: BindingMode.TwoWay
        );

    public static readonly StyledProperty<IEnumerable<Color>?> ItemsSourceProperty =
        AvaloniaProperty.Register<ColorSelectorControl, IEnumerable<Color>?>(nameof(ItemsSource));

    private bool bockSelectedColor;
    private bool bockSelectedItem;

    private SelectingItemsControl? selectingItemsControl;

    public Color? SelectedColor
    {
        get => GetValue(SelectedColorProperty);
        set => SetValue(SelectedColorProperty, value);
    }

    public IEnumerable<Color>? ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        var lb = e.NameScope.Find<SelectingItemsControl>("PART_SelectingItemsControl");
        UpdateSelectingItemsControl(ref selectingItemsControl, lb);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == SelectedColorProperty)
        {
            var icon = change.GetNewValue<Color?>();

            if (bockSelectedItem)
            {
                return;
            }

            bockSelectedItem = true;

            try
            {
                if (selectingItemsControl is not null)
                {
                    UpdateSelectedItem(selectingItemsControl, icon);
                }
            }
            finally
            {
                bockSelectedItem = false;
            }
        }
    }

    private void UpdateSelectingItemsControl(ref SelectingItemsControl? itemsControl, SelectingItemsControl? lb)
    {
        if (itemsControl is not null)
        {
            itemsControl.PropertyChanged -= SelectingItemsControlPropertyChanged;
        }

        itemsControl = lb;

        if (itemsControl is null)
        {
            return;
        }

        itemsControl.PropertyChanged += SelectingItemsControlPropertyChanged;
        UpdateSelectedItem(itemsControl, SelectedColor);
    }

    private void UpdateSelectedItem(SelectingItemsControl sic, Color? icon)
    {
        var selectedItem = sic.SelectedItem;

        if (icon is null)
        {
            if (sic.SelectedItem is not null)
            {
                sic.SelectedItem = null;
            }
        }
        else if (!icon.Equals(selectedItem))
        {
            sic.SelectedItem = icon;
        }
    }

    private void UpdateSelectedIcon(SelectingItemsControl itemsControl)
    {
        if (bockSelectedColor)
        {
            return;
        }

        bockSelectedColor = true;

        try
        {
            if (itemsControl.SelectedItem is null)
            {
                if (SelectedColor is not null)
                {
                    SelectedColor = null;
                }
            }
            else if (!itemsControl.SelectedItem.Equals(SelectedColor))
            {
                SelectedColor = itemsControl.SelectedItem as Color?;
            }
        }
        finally
        {
            bockSelectedColor = false;
        }
    }

    private void SelectingItemsControlPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (sender is not SelectingItemsControl itemsControl)
        {
            return;
        }

        if (e.Property == SelectingItemsControl.SelectedItemProperty)
        {
            UpdateSelectedIcon(itemsControl);
        }
    }
}