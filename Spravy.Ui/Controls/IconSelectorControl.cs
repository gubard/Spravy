namespace Spravy.Ui.Controls;

public class IconSelectorControl : TemplatedControl
{
    public static readonly StyledProperty<string?> SelectedIconProperty =
        AvaloniaProperty.Register<IconSelectorControl, string?>(
            nameof(SelectedIcon),
            defaultBindingMode: BindingMode.TwoWay
        );

    public static readonly StyledProperty<IEnumerable<string>?> ItemsSourceProperty =
        AvaloniaProperty.Register<IconSelectorControl, IEnumerable<string>?>(nameof(ItemsSource));

    public static readonly StyledProperty<IEnumerable<string>?> FavoriteItemsSourceProperty =
        AvaloniaProperty.Register<IconSelectorControl, IEnumerable<string>?>(nameof(FavoriteItemsSource));

    public static readonly StyledProperty<string?> SearchTextProperty =
        AvaloniaProperty.Register<IconSelectorControl, string?>(
            nameof(SearchText),
            defaultBindingMode: BindingMode.TwoWay
        );

    public static readonly DirectProperty<IconSelectorControl, IEnumerable> ItemsProperty =
        AvaloniaProperty.RegisterDirect<IconSelectorControl, IEnumerable>(nameof(Items), x => x.Items);

    private readonly AvaloniaList<string> items = new();
    private bool bockSelectedIcon;
    private bool bockSelectedItem;
    private SelectingItemsControl? favoriteSelectingItemsControl;
    private TextBox? searchTextBox;

    private SelectingItemsControl? selectingItemsControl;

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

    public IEnumerable<string>? FavoriteItemsSource
    {
        get => GetValue(FavoriteItemsSourceProperty);
        set => SetValue(FavoriteItemsSourceProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        var lb = e.NameScope.Find<SelectingItemsControl>("PART_SelectingItemsControl");
        var flb = e.NameScope.Find<SelectingItemsControl>("PART_FavoriteSelectingItemsControl");
        var tb = e.NameScope.Find<TextBox>("PART_SearchTextBox");
        UpdateSelectingItemsControl(ref selectingItemsControl, lb);
        UpdateSelectingItemsControl(ref favoriteSelectingItemsControl, flb);
        UpdateSearchTextBox(tb);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == SelectedIconProperty)
        {
            var icon = change.GetNewValue<string?>();

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

                if (favoriteSelectingItemsControl is not null)
                {
                    UpdateSelectedItem(favoriteSelectingItemsControl, icon);
                }
            }
            finally
            {
                bockSelectedItem = false;
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
        UpdateSelectedItem(itemsControl, SelectedIcon);
    }

    private void UpdateSelectedItem(SelectingItemsControl sic, string? icon)
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
        if (bockSelectedIcon)
        {
            return;
        }

        bockSelectedIcon = true;

        try
        {
            if (itemsControl.SelectedItem is null)
            {
                if (SelectedIcon is not null)
                {
                    SelectedIcon = null;
                }
            }
            else if (!itemsControl.SelectedItem.Equals(SelectedIcon))
            {
                SelectedIcon = itemsControl.SelectedItem.ToString();
            }
        }
        finally
        {
            bockSelectedIcon = false;
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