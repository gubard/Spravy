using Spravy.ToDo.Domain.Interfaces;
using Spravy.ToDo.Domain.Models;
using Spravy.Ui.Design.Helpers;
using Spravy.Ui.Design.Services;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Design.ViewModels;

public class ChangeToDoItemOrderIndexViewModelDesign : ChangeToDoItemOrderIndexViewModel
{
    public ChangeToDoItemOrderIndexViewModelDesign()
    {
        Mapper = ConstDesign.Mapper;

        ToDoService = new ToDoServiceDesign(
            Enumerable.Empty<IToDoSubItem>(),
            Enumerable.Empty<IToDoSubItem>(),
            new ToDoItemGroup(),
            new ToDoShortItem[]
            {
                new(Guid.NewGuid(), "ToDoShortItem")
            },
            Enumerable.Empty<IToDoSubItem>()
        );
    }
}