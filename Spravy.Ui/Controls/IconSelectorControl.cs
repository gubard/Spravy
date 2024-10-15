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

    public static readonly DirectProperty<IconSelectorControl, IEnumerable> CurrentItemsProperty =
        AvaloniaProperty.RegisterDirect<IconSelectorControl, IEnumerable>(
            nameof(CurrentItems),
            x => x.CurrentItems
        );

    private SelectingItemsControl? selectingItemsControl;
    private TextBox? searchTextBox;
    private AvaloniaList<string> currentItems = new();

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        var lb = e.NameScope.Find<SelectingItemsControl>("PART_SelectingItemsControl");
        var tb = e.NameScope.Find<TextBox>("PART_SearchTextBox");
        UpdateSelectingItemsControl(lb);
        UpdateSearchTextBox(tb);
    }

    public IEnumerable CurrentItems => currentItems;

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

        if (change.Property == ItemsSourceProperty)
        {
            var itemsSource = change.GetNewValue<IEnumerable<string>?>();
            UpdateCurrentItems(itemsSource, null);
        }
        else if (change.Property == SearchTextProperty)
        {
            if (ItemsSource is null)
            {
                return;
            }

            var searchText = change.GetNewValue<string?>();
            UpdateCurrentItems(ItemsSource, searchText);

            if (searchTextBox is not null)
            {
                searchTextBox.Text = searchText;
            }
        }
    }

    private void UpdateCurrentItems(IEnumerable<string>? items, string? searchText)
    {
        currentItems.Clear();

        if (items is null)
        {
            return;
        }

        if (searchText.IsNullOrWhiteSpace())
        {
            currentItems.AddRange(items);

            return;
        }

        var searchTextUpper = searchText.ToUpperInvariant();
        currentItems.AddRange(items.Where(x => x.ToUpperInvariant().Contains(searchTextUpper)));
    }

    private void UpdateSearchTextBox(TextBox? tb)
    {
        if (searchTextBox is not null)
        {
            searchTextBox.TextChanged -= SearchTextBoxTextChanged;
        }

        searchTextBox = tb;

        if (searchTextBox is not null)
        {
            searchTextBox.TextChanged += SearchTextBoxTextChanged;
            SearchText = searchTextBox.Text;
        }
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
