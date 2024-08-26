using Material.Icons.Avalonia;
using Microsoft.Extensions.Logging.Abstractions;

namespace Spravy.Ui.Controls;

public class ListControl : TemplatedControl
{
    public static readonly StyledProperty<ITemplate<Control>?> AddItemProperty =
        AvaloniaProperty.Register<ListControl, ITemplate<Control>?>(nameof(AddItem));

    public static readonly StyledProperty<IEnumerable?> ItemsSourceProperty =
        AvaloniaProperty.Register<ListControl, IEnumerable?>(nameof(ItemsSource));

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
                lc.UpdateDefaultItems();
                lc.UpdateNotifyCollectionChanged();
            }
        );
    }

    private IList? items;
    private IAddItem? addItem;
    private INotifyCollectionChanged? notifyCollectionChanged;
    private bool isEdit;

    public ITemplate<Control>? AddItem
    {
        get => GetValue(AddItemProperty);
        set => SetValue(AddItemProperty, value);
    }

    public IEnumerable? ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public IList? Items
    {
        get => items;
        private set => SetAndRaise(ItemsProperty, ref items, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        UpdateDefaultItems();
    }

    private void UpdateNotifyCollectionChanged()
    {
        if (notifyCollectionChanged is not null)
        {
            notifyCollectionChanged.CollectionChanged -= OnCollectionChanged;
        }

        if (ItemsSource is INotifyCollectionChanged n)
        {
            notifyCollectionChanged = n;
            notifyCollectionChanged.CollectionChanged += OnCollectionChanged;
        }
        else
        {
            notifyCollectionChanged = null;
        }
    }

    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (isEdit)
        {
            UpdateEditingItems();
        }
        else
        {
            UpdateDefaultItems();
        }
    }

    private void UpdateDefaultItems()
    {
        isEdit = false;

        if (ItemsSource is null)
        {
            Items = null;
        }
        else
        {
            var list = ItemsSource.OfType<object>().ToList();
            list.Add(CreateEditButton());
            Items = list;
        }
    }

    private void UpdateEditingItems()
    {
        isEdit = true;

        if (ItemsSource is null)
        {
            Items = null;
        }
        else
        {
            if (AddItem is null)
            {
                return;
            }

            var list = new List<object>(
                ItemsSource
                    .OfType<object>()
                    .Select(x =>
                    {
                        var item = new DeleteItemControl { Content = x };

                        item.ClickDelete += (_, _) =>
                        {
                            if (ItemsSource is IList l)
                            {
                                l.Remove(x);
                            }

                            UpdateEditingItems();
                        };

                        return (object)item;
                    })
            );

            var ai = AddItem.Build();

            if (ai is IAddItem add)
            {
                addItem = add;
            }

            list.Add(ai);
            list.Add(CreateAddButton());
            list.Add(CreateCancelButton());
            Items = list;
        }
    }

    private Button CreateEditButton()
    {
        var result = new Button { Content = new MaterialIcon { Kind = MaterialIconKind.Pencil } };
        result.Click += (_, _) => UpdateEditingItems();

        return result;
    }

    private Button CreateAddButton()
    {
        var result = new Button { Content = new MaterialIcon { Kind = MaterialIconKind.Plus } };

        result.Click += (_, _) =>
        {
            if (addItem is null)
            {
                return;
            }

            if (ItemsSource is IList list)
            {
                list.Add(addItem.Value);
            }

            UpdateEditingItems();
        };

        return result;
    }

    private Button CreateCancelButton()
    {
        var result = new Button { Content = new MaterialIcon { Kind = MaterialIconKind.Close } };
        result.Click += (_, _) => UpdateDefaultItems();

        return result;
    }
}
