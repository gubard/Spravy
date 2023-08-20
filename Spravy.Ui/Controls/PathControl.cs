using System;
using System.Collections.Specialized;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;

namespace Spravy.Ui.Controls;

[TemplatePart(ItemsControlPartName, typeof(ItemsControl))]
public class PathControl : TemplatedControl
{
    public const string ItemsControlPartName = "PART_ItemsControl";
    public const string DefaultSeparator = ">";

    public static readonly StyledProperty<AvaloniaList<object>?> ItemsProperty =
        AvaloniaProperty.Register<PathControl, AvaloniaList<object>?>(nameof(Items));

    public static readonly StyledProperty<IDataTemplate?> SeparatorProperty =
        AvaloniaProperty.Register<PathControl, IDataTemplate?>(nameof(Separator));

    private ItemsControl? itemsControl;

    public IDataTemplate? Separator
    {
        get => GetValue(SeparatorProperty);
        set => SetValue(SeparatorProperty, value);
    }

    public AvaloniaList<object>? Items
    {
        get => GetValue(ItemsProperty);
        set => UpdateItems(value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        itemsControl = e.NameScope.Get<ItemsControl>(ItemsControlPartName);
        UpdateItemsSource();
    }
    
    private void UpdateItems(AvaloniaList<object>? items)
    {
        if (Items is not null)
        {
            Items.CollectionChanged -= NotifyCollectionChangedEventHandler;
        }
        
        SetValue(ItemsProperty, items);

        if (items is not null)
        {
            items.CollectionChanged += NotifyCollectionChangedEventHandler;
        }

        UpdateItemsSource();
    }

    private void UpdateItemsSource()
    {
        if (itemsControl is null)
        {
            return;
        }

        if (Items is null)
        {
            itemsControl.ItemsSource = null;
            
            return;
        }
        
        if (Items.Count == 0)
        {
            itemsControl.ItemsSource = Array.Empty<object>();
            
            return;
        }

        var itemsSource = new object?[Items.Count + Items.Count - 1];

        for (var index = 0; index < Items.Count; index++)
        {
            itemsSource[index * 2] = Items[index];

            if (index == Items.Count - 1)
            {
                continue;
            }

            if (Separator is null)
            {
                itemsSource[index * 2 + 1] = null;
            }
            else
            {
                itemsSource[index * 2 + 1] = Separator.Build(null);
            }
        }

        itemsControl.ItemsSource = itemsSource;
    }

    private void NotifyCollectionChangedEventHandler(
        object? sender,
        NotifyCollectionChangedEventArgs e
    )
    {
        UpdateItemsSource();
    }
}