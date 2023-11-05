using Avalonia;
using Spravy.Domain.Extensions;
using Spravy.Schedule.Domain.Models;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.ToDo.Domain.Models;
using Spravy.Ui.Design.Helpers;
using Spravy.Ui.Design.Services;
using Spravy.Ui.Extensions;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Design.ViewModels;

public class ToDoItemHeaderViewModelDesign : ToDoItemHeaderViewModel
{
    public ToDoItemHeaderViewModelDesign()
    {
        var toDoService = new ToDoServiceDesign(
            Enumerable.Empty<IToDoSubItem>(),
            Enumerable.Empty<IToDoSubItem>(),
            new ToDoItemGroup(),
            Enumerable.Empty<ToDoShortItem>(),
            Enumerable.Empty<IToDoSubItem>()
        );

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
            Name = "Header",
            ScheduleService = new ScheduleServiceDesign(Enumerable.Empty<TimerItem>()),
        };
    }
}