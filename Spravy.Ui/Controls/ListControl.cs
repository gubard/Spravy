namespace Spravy.Ui.Controls;

public class ListControl : TemplatedControl
{
    public static readonly StyledProperty<ITemplate<AddItemControl>?> AddItemProperty =
        AvaloniaProperty.Register<ListControl, ITemplate<AddItemControl>?>(nameof(AddItem));

    public static readonly StyledProperty<IAvaloniaList<object>?> ItemsSourceProperty =
        AvaloniaProperty.Register<ListControl, IAvaloniaList<object>?>(nameof(ItemsSource));

    public static readonly DirectProperty<ListControl, IList?> ItemsProperty =
        AvaloniaProperty.RegisterDirect<ListControl, IList?>(
            nameof(Items),
            control => control.items
        );

    static ListControl()
    {
        ItemsSourceProperty.Changed.AddClassHandler<ListControl>(
            (lc, _) =>
            {
                var items = lc.ItemsSource?.ToList();
                items?.Add(lc.CreateAddButton());
                lc.Items = items;
            }
        );
    }

    private IList? items;

    public ITemplate<AddItemControl>? AddItem
    {
        get => GetValue(AddItemProperty);
        set => SetValue(AddItemProperty, value);
    }

    public IAvaloniaList<object>? ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public IList? Items
    {
        get => GetValue(ItemsProperty);
        private set => SetAndRaise(ItemsProperty, ref items, value);
    }

    private Button CreateAddButton()
    {
        var result = new Button();

        result.Click += (s, a) =>
        {
            if (Items is null)
            {
                return;
            }

            if (AddItem is null)
            {
                return;
            }

            Items.Remove(Items[^1]);
            Items.Add(AddItem.Build());
        };

        return result;
    }

    private Button CreateCompleteButton()
    {
        var result = new Button();

        result.Click += (s, a) =>
        {
            if (Items is null)
            {
                return;
            }

            if (AddItem is null)
            {
                return;
            }

            Items.Remove(Items[^1]);
            Items.Add(AddItem.Build());
        };

        return result;
    }
}
