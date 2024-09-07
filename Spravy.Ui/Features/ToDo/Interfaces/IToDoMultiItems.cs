namespace Spravy.Ui.Features.ToDo.Interfaces;

public interface IToDoMultiItems
{
    ToDoItemEntityNotify? Item { get; }
    ToDoSubItemsViewModel ToDoSubItemsViewModel { get; }
}
