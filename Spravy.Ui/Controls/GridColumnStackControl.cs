namespace Spravy.Ui.Controls;

public class GridColumnStackControl : TemplatedControl
{
    public static readonly StyledProperty<IEnumerable> ItemsSourceProperty =
        AvaloniaProperty.Register<GridColumnStackControl, IEnumerable>(nameof(ItemsSource));

    public static readonly StyledProperty<uint> ColumnCountProperty = AvaloniaProperty.Register<
        GridColumnStackControl,
        uint
    >(nameof(ColumnCount), 1);

    public static readonly StyledProperty<IDataTemplate?> ItemTemplateProperty =
        AvaloniaProperty.Register<GridColumnStackControl, IDataTemplate?>(nameof(ItemTemplate));

    static GridColumnStackControl()
    {
        ItemsSourceProperty.Changed.AddClassHandler<GridColumnStackControl>(
            (control, _) => control.UpdateGrid()
        );
        ColumnCountProperty.Changed.AddClassHandler<GridColumnStackControl>(
            (control, _) => control.UpdateGrid()
        );
    }

    public GridColumnStackControl()
    {
        ItemsSource = new AvaloniaList<object>();
    }

    private ItemsControl? itemsControl;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        itemsControl = e.NameScope.Find<ItemsControl>("PART_ItemsControl");
    }

    [Content]
    public IEnumerable ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    [InheritDataTypeFromItems(nameof(ItemsSource))]
    public IDataTemplate? ItemTemplate
    {
        get => GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    public uint ColumnCount
    {
        get => GetValue(ColumnCountProperty);
        set => SetValue(ColumnCountProperty, value);
    }

    private static Control ToControl(object item)
    {
        return item switch
        {
            Control c => c,
            { } i => new ContentControl { Content = i, },
        };
    }

    private void UpdateGrid()
    {
        if (itemsControl is null)
        {
            return;
        }

        if (itemsControl.ItemsPanelRoot is not Grid grid)
        {
            return;
        }

        grid.Children.Clear();
        grid.ColumnDefinitions.Clear();

        if (ItemsSource is null)
        {
            return;
        }

        var items = ItemsSource
            .OfType<object>()
            .Select(ToControl)
            .Where(x => x.IsVisible)
            .ToArray()
            .AsSpan();

        if (items.IsEmpty)
        {
            return;
        }

        if (items.Length == 1)
        {
            grid.ColumnDefinitions.Add(new() { Width = new(1, GridUnitType.Star), });

            Grid.SetColumn(items[0], 0);
            grid.Children.Add(items[0]);

            return;
        }

        grid.ColumnDefinitions.AddRange(
            Enumerable
                .Range(1, items.Length)
                .Select(_ => new ColumnDefinition { Width = new(1, GridUnitType.Star), })
        );

        if (items.Length > ColumnCount)
        {
            var stackPanels = new List<StackPanel>();

            for (var i = 0; i < ColumnCount; i++)
            {
                grid.ColumnDefinitions.Add(new() { Width = new(1, GridUnitType.Star), });

                var stackPanel = new StackPanel();
                Grid.SetColumn(stackPanel, i);
                stackPanels.Add(stackPanel);
            }

            var currentColumn = 0;

            for (var i = 0; i < items.Length; i++)
            {
                if (currentColumn == ColumnCount)
                {
                    currentColumn = 0;
                }

                stackPanels[currentColumn].Children.Add(ToControl(items[i]));
                currentColumn++;
            }
        }
        else
        {
            for (var i = 0; i < ColumnCount; i++)
            {
                grid.ColumnDefinitions.Add(new() { Width = new(1, GridUnitType.Star), });
            }

            for (var i = 0; i < items.Length; i++)
            {
                Grid.SetColumn(items[i], i);
            }

            grid.Children.AddRange(items.ToArray());
        }
    }
}
