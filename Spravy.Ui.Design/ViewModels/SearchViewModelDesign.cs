using Spravy.ToDo.Domain.Interfaces;
using Spravy.ToDo.Domain.Models;
using Spravy.Ui.Design.Helpers;
using Spravy.Ui.Design.Services;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Design.ViewModels;

public class SearchViewModelDesign : SearchViewModel
{
    public SearchViewModelDesign()
    {
        ToDoSubItemsViewModel = new()
        {
            Mapper = ConstDesign.Mapper,
            Navigator = ConstDesign.Navigator,
            DialogViewer = ConstDesign.DialogViewer,
            OpenerLink = ConstDesign.OpenerLink,
            ToDoService = new ToDoServiceDesign(
                Enumerable.Empty<IToDoSubItem>(),
                Enumerable.Empty<IToDoSubItem>(),
                null,
                Enumerable.Empty<ToDoShortItem>(),
                Enumerable.Empty<IToDoSubItem>()
            ),
            MultiEditingItemsViewModel = null,
            List = null,
        };
    }
}