using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.VisualTree;
using FluentAssertions;
using Spravy.Domain.Extensions;
using Spravy.Integration.Tests.Helpers;
using Spravy.Ui.Features.ToDo.Views;
using Spravy.Ui.ViewModels;
using Spravy.Ui.Views;
using SukiUI.Controls;

namespace Spravy.Integration.Tests.Extensions;

public static class RootToDoItemsViewExtension
{
    public static RootToDoItemsView AddToDoItem(this RootToDoItemsView view, Window window, string name)
    {
        var toDoItemCount = view.GetDoItemItemsControl()?.ItemCount ?? 0;

        view.Case(() => view.FindControl<Button>(ElementNames.AddRootToDoItemButton)
                .ThrowIfNull()
                .ClickOn(window)
                .RunJobsAll(2))
            .Case(() => window.GetContentDialogView<ConfirmView, ConfirmViewModel>()
                .Case(c => c.FindControl<ContentControl>(ElementNames.ContentContentControl)
                    .ThrowIfNull()
                    .GetContentView<AddRootToDoItemView>()
                    .FindControl<ContentControl>(ElementNames.ToDoItemContentContentControl)
                    .ThrowIfNull()
                    .GetContentView<ToDoItemContentView>()
                    .FindControl<TextBox>(ElementNames.NameTextBox)
                    .ThrowIfNull()
                    .ClearText(window)
                    .SetText(window, name))
                .Case(c => c.FindControl<Button>(ElementNames.OkButton)
                    .ThrowIfNull()
                    .ClickOn(window)
                    .RunJobsAll(5)))
            .GetDoItemItemsControl()
            .ThrowIfNull()
            .Case(ic => ic.ItemCount.Should().Be(toDoItemCount + 1))
            .GetVisualChildren()
            .Single()
            .ThrowIfIsNotCast<Border>()
            .Child
            .ThrowIfNull()
            .ThrowIfIsNotCast<ItemsPresenter>()
            .GetVisualChildren()
            .Single()
            .ThrowIfIsNotCast<VirtualizingStackPanel>()
            .Children
            .Last()
            .ThrowIfIsNotCast<ContentPresenter>()
            .Child
            .ThrowIfNull()
            .ThrowIfIsNotCast<BusyArea>()
            .Content
            .ThrowIfNull()
            .ThrowIfIsNotCast<Button>()
            .Content
            .ThrowIfNull()
            .ThrowIfIsNotCast<Grid>()
            .Children
            .ElementAt(1)
            .ThrowIfIsNotCast<StackPanel>()
            .Children
            .First()
            .ThrowIfIsNotCast<Grid>()
            .Children
            .ElementAt(1)
            .ThrowIfIsNotCast<TextBlock>()
            .Text
            .Should()
            .Be(name);

        return view;
    }

    public static ItemsControl? GetDoItemItemsControl(this RootToDoItemsView view)
    {
        var children = view.FindControl<ContentControl>(ElementNames.ToDoSubItemsContentControl)
            .ThrowIfNull()
            .GetContentView<ToDoSubItemsView>()
            .FindControl<ContentControl>(ElementNames.ListContentControl)
            .ThrowIfNull()
            .GetContentView<MultiToDoItemsView>()
            .FindControl<ContentControl>(ElementNames.ContentContentControl)
            .ThrowIfNull()
            .GetContentView<ToDoItemsGroupByView>()
            .FindControl<ContentControl>(ElementNames.ContentContentControl)
            .ThrowIfNull()
            .GetContentView<ToDoItemsGroupByStatusView>()
            .FindControl<ContentControl>(ElementNames.ReadyForCompletedContentControl)
            .ThrowIfNull()
            .GetVisualChildren()
            .ToArray();

        if (children.Any())
        {
            return children.Single()
                .GetVisualChildren()
                .Single()
                .ThrowIfIsNotCast<ToDoItemsView>()
                .FindControl<ItemsControl>(ElementNames.ItemsItemsControl)
                .ThrowIfNull();
        }

        return null;
    }
}