namespace Spravy.Ui.Controls;

public class IconSelectorControl : TemplatedControl
{
    public static readonly StyledProperty<string?> SelectedIconProperty = AvaloniaProperty.Register<
        IconSelectorControl,
        string?
    >(nameof(SelectedIcon), defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<IEnumerable<string>?> ItemsSourceProperty =
        AvaloniaProperty.Register<IconSelectorControl, IEnumerable<string>?>(nameof(ItemsSource));

    public static readonly StyledProperty<string?> SearchTextProperty = AvaloniaProperty.Register<
        IconSelectorControl,
        string?
    >(nameof(SearchText), defaultBindingMode: BindingMode.TwoWay);

    public static readonly DirectProperty<IconSelectorControl, IEnumerable> ItemsProperty =
        AvaloniaProperty.RegisterDirect<IconSelectorControl, IEnumerable>(
            nameof(Items),
            x => x.Items
        );

    private SelectingItemsControl? selectingItemsControl;
    private TextBox? searchTextBox;
    private readonly AvaloniaList<string> items = new();

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        var lb = e.NameScope.Find<SelectingItemsControl>("PART_SelectingItemsControl");
        var tb = e.NameScope.Find<TextBox>("PART_SearchTextBox");
        UpdateSelectingItemsControl(lb);
        UpdateSearchTextBox(tb);
    }

    public IEnumerable Items => items;

    public string? SearchText
    {
        get => GetValue(SearchTextProperty);
        set => SetValue(SearchTextProperty, value);
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

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == SelectedIconProperty)
        {
            if (selectingItemsControl is null)
            {
                return;
            }

            var icon = change.GetNewValue<string?>();
            var selectedItem = selectingItemsControl.SelectedItem;

            if (icon is null && selectedItem is not null)
            {
                selectingItemsControl.SelectedItem = null;
            }
            else if (selectedItem is string item && item != icon)
            {
                selectingItemsControl.SelectedItem = icon;
            }
        }
        else if (change.Property == ItemsSourceProperty)
        {
            var itemsSource = change.GetNewValue<IEnumerable<string>?>();
            UpdateItems(itemsSource, null);
        }
        else if (change.Property == SearchTextProperty)
        {
            var searchText = change.GetNewValue<string?>();
            UpdateItems(ItemsSource, searchText);

            if (searchTextBox is null)
            {
                return;
            }

            if (searchTextBox.Text != searchText)
            {
                searchTextBox.Text = searchText;
            }
        }
    }

    private void UpdateItems(IEnumerable<string>? currentItems, string? searchText)
    {
        items.Clear();

        if (currentItems is null)
        {
            return;
        }

        if (searchText.IsNullOrWhiteSpace())
        {
            items.AddRange(currentItems);

            return;
        }

        var searchTextUpper = searchText.ToUpperInvariant();
        items.AddRange(currentItems.Where(x => x.ToUpperInvariant().Contains(searchTextUpper)));
    }

    private void UpdateSearchTextBox(TextBox? tb)
    {
        if (searchTextBox is not null)
        {
            searchTextBox.TextChanged -= SearchTextBoxTextChanged;
        }

        searchTextBox = tb;

        if (searchTextBox is null)
        {
            return;
        }

        searchTextBox.TextChanged += SearchTextBoxTextChanged;
        SearchText = searchTextBox.Text;
    }

    private void UpdateSelectingItemsControl(SelectingItemsControl? lb)
    {
        if (selectingItemsControl is not null)
        {
            selectingItemsControl.PropertyChanged -= SelectingItemsControlPropertyChanged;
        }

        selectingItemsControl = lb;

        if (selectingItemsControl is null)
        {
            return;
        }

        selectingItemsControl.PropertyChanged += SelectingItemsControlPropertyChanged;
        UpdateSelectedItem(selectingItemsControl);
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

    private void SearchTextBoxTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (sender is not TextBox textBox)
        {
            return;
        }

        var text = textBox.Text;

        if (SearchText != text)
        {
            SearchText = text;
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
