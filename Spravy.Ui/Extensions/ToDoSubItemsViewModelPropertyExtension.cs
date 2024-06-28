namespace Spravy.Ui.Extensions;

public static class ToDoSubItemsViewModelPropertyExtension
{
    public static Result<ReadOnlyMemory<ToDoItemEntityNotify>> GetSelectedItems(
        this IToDoSubItemsViewModelProperty property
    )
    {
        ReadOnlyMemory<ToDoItemEntityNotify> selected = property
            .ToDoSubItemsViewModel.List.ToDoItems.GroupByNone.Items.Items.Where(x => x.IsSelected)
            .ToArray();

        if (selected.IsEmpty)
        {
            return new(new NonItemSelectedError());
        }

        return new(selected);
    }
}
