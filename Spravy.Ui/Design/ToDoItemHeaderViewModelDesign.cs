using System.Linq;
using Avalonia;
using Spravy.Domain.Extensions;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.Ui.Extensions;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Design;

public class ToDoItemHeaderViewModelDesign : ToDoItemHeaderViewModel
{
    public ToDoItemHeaderViewModelDesign()
    {
        var toDoService = new ToDoServiceDesign(Enumerable.Empty<IToDoSubItem>(), Enumerable.Empty<IToDoSubItem>());

        Item = new ToDoItemGroupViewModel
        {
            DialogViewer = ConstDesign.DialogViewer,
            Clipboard = Application.Current
                                .ThrowIfNull("Application")
                                .GetTopLevel()
                                .ThrowIfNull("TopLevel")
                                .Clipboard
                                .ThrowIfNull(),
            Mapper = ConstDesign.Mapper,
            Navigator = ConstDesign.Navigator,
            ToDoService = toDoService,
            ToDoItemHeaderView = new(),
            Path = new(),
            ToDoSubItemsViewModel = new ToDoSubItemsViewModelDesign
            {
                DialogViewer = ConstDesign.DialogViewer,
                Mapper = ConstDesign.Mapper,
                Navigator = ConstDesign.Navigator,
                ToDoService = toDoService
            },
            Name = "Header"
        };
    }
}