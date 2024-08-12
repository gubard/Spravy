namespace Spravy.Ui.Features.ToDo.Interfaces;

public interface IToDoSubItemsViewModelProperty
{
    ToDoItemEntityNotify Item { get; }
    ToDoSubItemsViewModel ToDoSubItemsViewModel { get; }
}
