namespace Spravy.Ui.Extensions;

public static class ToDoSubItemsViewModelPropertyExtension
{
    public static Result<ReadOnlyMemory<ToDoItemEntityNotify>> GetSelectedItems(
        this IToDoSubItemsViewModelProperty property
    )
    {
        var selected = property
            .ToDoSubItemsViewModel.List.Items.Items.Where(x => x.IsSelected)
            .ToArray()
            .ToReadOnlyMemory();

        if (selected.IsEmpty)
        {
            return new(new NonItemSelectedError());
        }

        return new(selected);
    }

    public static Result<ReadOnlyMemory<ToDoItemEntityNotify>> GetNotSelectedItems(
        this IToDoSubItemsViewModelProperty property
    )
    {
        var selected = property
            .ToDoSubItemsViewModel.List.Items.Items.Where(x => !x.IsSelected)
            .ToArray()
            .ToReadOnlyMemory();

        if (selected.IsEmpty)
        {
            return new(new AllItemSelectedError());
        }

        return new(selected);
    }
}
