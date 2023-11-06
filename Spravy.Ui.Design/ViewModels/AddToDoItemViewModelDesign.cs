using Spravy.ToDo.Domain.Interfaces;
using Spravy.ToDo.Domain.Models;
using Spravy.Ui.Design.Services;
using Spravy.Ui.Models;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Design.ViewModels;

public class AddToDoItemViewModelDesign : AddToDoItemViewModel
{
    public AddToDoItemViewModelDesign()
    {
        Parent = new ToDoSubItemGroupNotify();
        Path = new();

        ToDoService = new ToDoServiceDesign(
            Enumerable.Empty<IToDoSubItem>(),
            Enumerable.Empty<IToDoSubItem>(),
            new ToDoItemGroup(
                Guid.NewGuid(),
                "Group",
                Array.Empty<IToDoSubItem>(),
                Array.Empty<ToDoItemParent>(),
                "Description",
                false,
                null
            ),
            Enumerable.Empty<ToDoShortItem>(),
            Enumerable.Empty<IToDoSubItem>()
        );
    }
}