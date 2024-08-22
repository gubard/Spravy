namespace Spravy.Ui.Controls;

public class ListControl : TemplatedControl
{
    public static readonly StyledProperty<IAddItem?> AddItemProperty = AvaloniaProperty.Register<
        ListControl,
        IAddItem?
    >(nameof(AddItem));

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
            (lc, _) => lc.Items = lc.ItemsSource?.ToList()
        );
    }

    private IList? items;

    public IAddItem? AddItem
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
}
