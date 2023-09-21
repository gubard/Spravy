using System;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactions.DragAndDrop;
using Spravy.Ui.Extensions;
using Spravy.Ui.Models;

namespace Spravy.Ui.Handlers;

public class ItemsListBoxDropHandler<TItem> : DropHandlerBase
{
    public event EventHandler<MovedDragDropArgs<TItem>>? Moved;

    private bool Validate(
        ListBox listBox,
        DragEventArgs e,
        object? sourceContext,
        object? targetContext,
        bool bExecute,
        ListBoxItem item,
        Point pointerPosition
    )
    {
        if (sourceContext is not TItem sourceItem
            || listBox.ItemsSource is not IAvaloniaList<TItem> items
            || listBox.GetVisualAt(e.GetPosition(listBox)) is not Control targetControl
            || targetControl.DataContext is not TItem targetItem)
        {
            return false;
        }

        var sourceIndex = items.IndexOf(sourceItem);
        var targetIndex = items.IndexOf(targetItem);

        if (sourceIndex < 0 || targetIndex < 0)
        {
            return false;
        }

        switch (e.DragEffects)
        {
            case DragDropEffects.Move:
            {
                if (bExecute)
                {
                    Moved?.Invoke(
                        targetContext,
                        new MovedDragDropArgs<TItem>(
                            sourceItem,
                            sourceIndex,
                            targetItem,
                            targetIndex,
                            item,
                            pointerPosition
                        )
                    );
                }

                return true;
            }
            default:
                return false;
        }
    }

    public override bool Validate(
        object? sender,
        DragEventArgs e,
        object? sourceContext,
        object? targetContext,
        object? state
    )
    {
        if (e.Source is Control control && sender is ListBox listBox)
        {
            if (control is not ListBoxItem item)
            {
                var parent = control.FindParent<ListBoxItem>();

                if (parent is null)
                {
                    return false;
                }

                item = parent;
            }

            var pointerPosition = e.GetPosition(item);

            return Validate(listBox, e, sourceContext, targetContext, false, item, pointerPosition);
        }

        return false;
    }

    public override bool Execute(
        object? sender,
        DragEventArgs e,
        object? sourceContext,
        object? targetContext,
        object? state
    )
    {
        if (e.Source is Control control && sender is ListBox listBox)
        {
            if (control is not ListBoxItem item)
            {
                var parent = control.FindParent<ListBoxItem>();

                if (parent is null)
                {
                    return false;
                }

                item = parent;
            }

            var pointerPosition = e.GetPosition(item);

            return Validate(listBox, e, sourceContext, targetContext, true, item, pointerPosition);
        }

        return false;
    }
}