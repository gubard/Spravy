using Spravy.ToDo.Domain.Interfaces;
using Spravy.ToDo.Domain.Models;
using Spravy.Ui.Design.Helpers;
using Spravy.Ui.Design.Services;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Design.ViewModels;

public class RootToDoItemsViewModelDesign : RootToDoItemsViewModel
{
    public RootToDoItemsViewModelDesign()
    {
        var toDoServiceDesign = new ToDoServiceDesign(
            new IToDoSubItem[]
            {
            },
          new IToDoSubItem[]
          {
          },
            null,
            Enumerable.Empty<ToDoShortItem>(),
            Enumerable.Empty<IToDoSubItem>()
        );

        ToDoService = toDoServiceDesign;

        ToDoSubItemsViewModel = new ToDoSubItemsViewModelDesign
        {
            Mapper = ConstDesign.Mapper,
            Navigator = ConstDesign.Navigator,
            DialogViewer = ConstDesign.DialogViewer,
            ToDoService = toDoServiceDesign,
            List = null,
        };
    }
}