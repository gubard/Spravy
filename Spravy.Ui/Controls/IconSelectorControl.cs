namespace Spravy.Ui.Controls;

public class IconSelectorControl : TemplatedControl
{
    public static readonly StyledProperty<string?> SelectedIconProperty = AvaloniaProperty.Register<
        IconSelectorControl,
        string?
    >(nameof(SelectedIcon), defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<IEnumerable<string>?> ItemsSourceProperty =
        AvaloniaProperty.Register<IconSelectorControl, IEnumerable<string>?>(nameof(ItemsSource));

    private SelectingItemsControl? selectingItemsControl;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        var lb = e.NameScope.Find<SelectingItemsControl>("PART_SelectingItemsControl");
        UpdateSelectingItemsControl(lb);
    }

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

    private void UpdateSelectingItemsControl(SelectingItemsControl? lb)
    {
        if (selectingItemsControl is not null)
        {
            selectingItemsControl.PropertyChanged -= SelectingItemsControlPropertyChanged;
        }

        selectingItemsControl = lb;

        if (selectingItemsControl is not null)
        {
            selectingItemsControl.PropertyChanged += SelectingItemsControlPropertyChanged;
            UpdateSelectedItem(selectingItemsControl);
        }
    }

    private void UpdateSelectedItem(SelectingItemsControl itemsControl)
    {
        if (itemsControl.SelectedItem is null && SelectedIcon is not null)
        {
            SelectedIcon = null;
        }
        else if (itemsControl.SelectedItem is string item && item != SelectedIcon)
        {
            SelectedIcon = item;
        }
    }

    private void SelectingItemsControlPropertyChanged(
        object? sender,
        AvaloniaPropertyChangedEventArgs e
    )
    {
        if (sender is not SelectingItemsControl itemsControl)
        {
            return;
        }

        if (e.Property == SelectingItemsControl.SelectedItemProperty)
        {
            UpdateSelectedItem(itemsControl);
        }
    }
}
