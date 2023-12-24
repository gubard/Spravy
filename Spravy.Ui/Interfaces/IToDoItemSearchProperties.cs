using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Interfaces;

public interface IToDoItemSearchProperties : IRefresh
{
    ToDoSubItemsViewModel ToDoSubItemsViewModel { get; }
    string SearchText { get; }
}