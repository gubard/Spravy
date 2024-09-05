namespace Spravy.Ui.Features.ToDo.Interfaces;

public interface IToDoMultiItems
{
    Option<ToDoItemEntityNotify> Item { get; }
    ToDoSubItemsViewModel ToDoSubItemsViewModel { get; }
}
