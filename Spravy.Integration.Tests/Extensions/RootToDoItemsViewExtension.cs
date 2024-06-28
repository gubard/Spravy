namespace Spravy.Integration.Tests.Extensions;

public static class RootToDoItemsViewExtension
{
    public static MenuItem GetToDoItemReset(this RootToDoItemsView view, Index index)
    {
        return view.GetToDoItemDotsStackPanel(index)
            .Children.ElementAt(9)
            .ThrowIfIsNotCast<MenuItem>();
    }

    public static MenuItem GetToDoItemReorder(this RootToDoItemsView view, Index index)
    {
        return view.GetToDoItemDotsStackPanel(index)
            .Children.ElementAt(8)
            .ThrowIfIsNotCast<MenuItem>();
    }

    public static StackPanel GetToDoItemDotsStackPanel(this RootToDoItemsView view, Index index)
    {
        return view.GetToDoItemDotsButton(index)
            .Flyout.ThrowIfNull()
            .ThrowIfIsNotCast<IPopupHostProvider>()
            .PopupHost.ThrowIfNull()
            .ThrowIfIsNotCast<Visual>()
            .GetVisualChildren()
            .Single()
            .ThrowIfIsNotCast<LayoutTransformControl>()
            .Child.ThrowIfNull()
            .ThrowIfIsNotCast<VisualLayerManager>()
            .Child.ThrowIfNull()
            .ThrowIfIsNotCast<ContentPresenter>()
            .Child.ThrowIfNull()
            .ThrowIfIsNotCast<MenuFlyoutPresenter>()
            .GetVisualChildren()
            .Single()
            .ThrowIfIsNotCast<Panel>()
            .Children.Last()
            .ThrowIfIsNotCast<Border>()
            .Child.ThrowIfNull()
            .ThrowIfIsNotCast<ItemsPresenter>()
            .GetVisualChildren()
            .Single()
            .ThrowIfIsNotCast<StackPanel>();
    }

    public static Button GetToDoItemDotsButton(this RootToDoItemsView view, Index index)
    {
        return view.GetToDoItemButton(index)
            .Content.ThrowIfNull()
            .ThrowIfIsNotCast<Grid>()
            .Children.Last()
            .ThrowIfIsNotCast<Grid>()
            .Children.First()
            .ThrowIfIsNotCast<Button>();
    }

    public static Button GetToDoItemButton(this RootToDoItemsView view, Index index)
    {
        return view.GetDoItemItemsControl()
            .ThrowIfNull()
            .GetVisualChildren()
            .Single()
            .ThrowIfIsNotCast<Border>()
            .Child.ThrowIfNull()
            .ThrowIfIsNotCast<ItemsPresenter>()
            .GetVisualChildren()
            .Single()
            .ThrowIfIsNotCast<StackPanel>()
            .Children.ElementAt(index)
            .ThrowIfIsNotCast<ContentPresenter>()
            .Child.ThrowIfNull()
            .ThrowIfIsNotCast<Button>();
    }

    public static RootToDoItemsView AddToDoItem(
        this RootToDoItemsView view,
        Window window,
        string name
    )
    {
        var toDoItemCount = view.GetDoItemItemsControl()?.ItemCount ?? 0;

        view.Case(
                () =>
                    view.GetControl<Button>(ElementNames.AddRootToDoItemButton)
                        .ClickOn(window)
                        .RunJobsAll(4)
            )
            .Case(
                () =>
                    window
                        .GetContentDialogView<ConfirmView, ConfirmViewModel>()
                        .Case(c =>
                            c.GetControl<ContentControl>(ElementNames.ContentContentControl)
                                .GetContentView<AddRootToDoItemView>()
                                .GetControl<ContentControl>(
                                    ElementNames.ToDoItemContentContentControl
                                )
                                .GetContentView<ToDoItemContentView>()
                                .GetControl<TextBox>(ElementNames.NameTextBox)
                                .ClearText(window)
                                .SetText(window, name)
                        )
                        .Case(c =>
                            c.GetControl<Button>(ElementNames.OkButton)
                                .ClickOn(window)
                                .RunJobsAll(5)
                        )
            )
            .GetDoItemItemsControl()
            .ThrowIfNull()
            .Case(ic => ic.ItemCount.Should().Be(toDoItemCount + 1));

        view.RunJobsAll(4).CheckLastToDoItemName(name);

        return view;
    }

    public static RootToDoItemsView CheckLastToDoItemName(this RootToDoItemsView view, string name)
    {
        view.GetDoItemItemsControl()
            .ThrowIfNull()
            .GetVisualChildren()
            .Single()
            .ThrowIfIsNotCast<Border>()
            .Child.ThrowIfNull()
            .ThrowIfIsNotCast<ItemsPresenter>()
            .GetVisualChildren()
            .Single()
            .ThrowIfIsNotCast<StackPanel>()
            .Children.Last()
            .ThrowIfIsNotCast<ContentPresenter>()
            .Child.ThrowIfNull()
            .ThrowIfIsNotCast<Button>()
            .Content.ThrowIfNull()
            .ThrowIfIsNotCast<Grid>()
            .Children.ElementAt(2)
            .ThrowIfIsNotCast<StackPanel>()
            .Children.First()
            .ThrowIfIsNotCast<TextBlock>()
            .Case(tb => tb.Text.Should().Be(name));

        return view;
    }

    public static ItemsControl? GetDoItemItemsControl(this RootToDoItemsView view)
    {
        var children = view.GetControl<ContentControl>(ElementNames.ToDoSubItemsContentControl)
            .GetContentView<ToDoSubItemsView>()
            .GetControl<ContentControl>(ElementNames.ListContentControl)
            .GetContentView<MultiToDoItemsView>()
            .GetControl<ContentControl>(ElementNames.ContentContentControl)
            .GetContentView<ToDoItemsGroupByView>()
            .GetControl<ContentControl>(ElementNames.ContentContentControl)
            .GetContentView<ToDoItemsGroupByStatusView>()
            .GetControl<ContentControl>(ElementNames.ReadyForCompletedContentControl)
            .GetVisualChildren()
            .ToArray();

        if (children.Any())
        {
            return children
                .Single()
                .GetVisualChildren()
                .Single()
                .ThrowIfIsNotCast<ToDoItemsView>()
                .GetControl<ItemsControl>(ElementNames.ItemsItemsControl);
        }

        return null;
    }
}
