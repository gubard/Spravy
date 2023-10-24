using System;
using System.Linq;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.ToDo.Domain.Models;
using Spravy.Ui.ViewModels;
using Spravy.Ui.Views;

namespace Spravy.Ui.Design;

public class RootToDoItemViewModelDesign : RootToDoItemViewModel
{
    public RootToDoItemViewModelDesign()
    {
        var toDoServiceDesign = new ToDoServiceDesign(
            new IToDoSubItem[]
            {
                new ToDoSubItemGroup(
                    Guid.NewGuid(),
                    "Group",
                    1,
                    ToDoItemStatus.Completed,
                    string.Empty,
                    false,
                    null
                ),
                new ToDoSubItemPeriodicity(
                    Guid.NewGuid(),
                    "Periodicity",
                    1,
                    ToDoItemStatus.Completed,
                    string.Empty,
                    false,
                    DateTimeOffset.Now,
                    null,
                    1,
                    1,
                    1,
                    null
                ),
                new ToDoSubItemPlanned(
                    Guid.NewGuid(),
                    "Planned",
                    1u,
                    ToDoItemStatus.Completed,
                    string.Empty,
                    false,
                    null,
                    DateTimeOffset.Now,
                    1u,
                    1u,
                    1u,
                    false,
                    null
                ),
                new ToDoSubItemValue(
                    Guid.NewGuid(),
                    "Value",
                    false,
                    1,
                    ToDoItemStatus.Completed,
                    string.Empty,
                    1u,
                    1u,
                    1u,
                    false,
                    null,
                    null
                ),
                new ToDoSubItemPeriodicityOffset(
                    Guid.NewGuid(),
                    "PeriodicityOffset",
                    1u,
                    ToDoItemStatus.Completed,
                    string.Empty,
                    false,
                    DateTimeOffset.Now,
                    null,
                    1,
                    1,
                    1,
                    null
                ),
                //------------------------------------------------------------------------------------------------------
                new ToDoSubItemGroup(
                    Guid.NewGuid(),
                    "Group LastCompleted",
                    1,
                    ToDoItemStatus.Completed,
                    string.Empty,
                    false,
                    null
                ),
                new ToDoSubItemPeriodicity(
                    Guid.NewGuid(),
                    "Periodicity LastCompleted",
                    1,
                    ToDoItemStatus.Completed,
                    string.Empty,
                    false,
                    DateTimeOffset.Now,
                    null,
                    1,
                    1,
                    1,
                    DateTimeOffset.Now
                ),
                new ToDoSubItemPlanned(
                    Guid.NewGuid(),
                    "Planned LastCompleted",
                    1u,
                    ToDoItemStatus.Completed,
                    string.Empty,
                    false,
                    null,
                    DateTimeOffset.Now,
                    1u,
                    1u,
                    1u,
                    false,
                    DateTimeOffset.Now
                ),
                new ToDoSubItemValue(
                    Guid.NewGuid(),
                    "Value LastCompleted",
                    false,
                    1,
                    ToDoItemStatus.Completed,
                    string.Empty,
                    1u,
                    1u,
                    1u,
                    false,
                    null,
                    DateTimeOffset.Now
                ),
                new ToDoSubItemPeriodicityOffset(
                    Guid.NewGuid(),
                    "PeriodicityOffset LastCompleted",
                    1u,
                    ToDoItemStatus.Completed,
                    string.Empty,
                    false,
                    DateTimeOffset.Now,
                    null,
                    1,
                    1,
                    1,
                    DateTimeOffset.Now
                ),
            },
            Enumerable.Empty<IToDoSubItem>()
        );

        ToDoService = toDoServiceDesign;
        Mapper = ConstDesign.Mapper;

        ToDoSubItemsView = new ToDoSubItemsView
        {
            ViewModel = new ToDoSubItemsViewModelDesign
            {
                Mapper = ConstDesign.Mapper,
                Navigator = ConstDesign.Navigator,
                DialogViewer = ConstDesign.DialogViewer,
                ToDoService = toDoServiceDesign,
            },
        };
    }
}