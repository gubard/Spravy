using Spravy.ToDo.Domain.Interfaces;
using Spravy.ToDo.Domain.Models;
using Spravy.Ui.Design.Helpers;
using Spravy.Ui.Design.Services;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Design.ViewModels;

public class LeafToDoItemsViewModelDesign : LeafToDoItemsViewModel
{
    public LeafToDoItemsViewModelDesign()
    {
        var toDoServiceDesign = new ToDoServiceDesign(
            Enumerable.Empty<IToDoSubItem>(),
            new IToDoSubItem[]
            {
         
            },
            null,
            Enumerable.Empty<ToDoShortItem>(),
            new IToDoSubItem[]
            {
               
            }
        );

        ToDoService = toDoServiceDesign;
        Mapper = ConstDesign.Mapper;

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