using Spravy.ToDo.Domain.Enums;
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
                new ToDoSubItemGroup(
                    Guid.NewGuid(),
                    "Favorite Group",
                    1,
                    ToDoItemStatus.Miss,
                    string.Empty,
                    true,
                    null,
                    null
                ),
            },
            new ToDoItemGroup(),
            Enumerable.Empty<ToDoShortItem>(),
            new IToDoSubItem[]
            {
                new ToDoSubItemGroup(
                    Guid.NewGuid(),
                    "Group",
                    1,
                    ToDoItemStatus.Miss,
                    string.Empty,
                    false,
                    null,
                    null
                ),
                new ToDoSubItemPeriodicity(
                    Guid.NewGuid(),
                    "Periodicity",
                    1,
                    ToDoItemStatus.Miss,
                    string.Empty,
                    false,
                    DateTimeOffset.Now,
                    null,
                    100,
                    100,
                    100,
                    null,
                    null
                ),
                new ToDoSubItemPlanned(
                    Guid.NewGuid(),
                    "Planned",
                    1u,
                    ToDoItemStatus.Miss,
                    string.Empty,
                    false,
                    null,
                    DateTimeOffset.Now,
                    100u,
                    100u,
                    100u,
                    false,
                    null,
                    null
                ),
                new ToDoSubItemValue(
                    Guid.NewGuid(),
                    "Value",
                    false,
                    1,
                    ToDoItemStatus.Miss,
                    string.Empty,
                    100u,
                    100u,
                    100u,
                    false,
                    null,
                    null,
                    null
                ),
                new ToDoSubItemPeriodicityOffset(
                    Guid.NewGuid(),
                    "PeriodicityOffset",
                    1u,
                    ToDoItemStatus.Miss,
                    string.Empty,
                    false,
                    DateTimeOffset.Now,
                    null,
                    100,
                    100,
                    100,
                    null,
                    null
                ),

                //-------------------------------------------LastCompleted----------------------------------------------
                new ToDoSubItemGroup(
                    Guid.NewGuid(),
                    "Group LastCompleted",
                    1,
                    ToDoItemStatus.ReadyForComplete,
                    string.Empty,
                    false,
                    null,
                    null
                ),
                new ToDoSubItemPeriodicity(
                    Guid.NewGuid(),
                    "Periodicity LastCompleted",
                    1,
                    ToDoItemStatus.ReadyForComplete,
                    string.Empty,
                    false,
                    DateTimeOffset.Now,
                    null,
                    1,
                    1,
                    1,
                    DateTimeOffset.Now,
                    null
                ),
                new ToDoSubItemPlanned(
                    Guid.NewGuid(),
                    "Planned LastCompleted",
                    1u,
                    ToDoItemStatus.ReadyForComplete,
                    string.Empty,
                    false,
                    null,
                    DateTimeOffset.Now,
                    1u,
                    1u,
                    1u,
                    false,
                    DateTimeOffset.Now,
                    null
                ),
                new ToDoSubItemValue(
                    Guid.NewGuid(),
                    "Value LastCompleted",
                    false,
                    1,
                    ToDoItemStatus.ReadyForComplete,
                    string.Empty,
                    1u,
                    1u,
                    1u,
                    false,
                    null,
                    DateTimeOffset.Now,
                    null
                ),
                new ToDoSubItemPeriodicityOffset(
                    Guid.NewGuid(),
                    "PeriodicityOffset LastCompleted",
                    1u,
                    ToDoItemStatus.ReadyForComplete,
                    string.Empty,
                    false,
                    DateTimeOffset.Now,
                    null,
                    1,
                    1,
                    1,
                    DateTimeOffset.Now,
                    null
                ),

                //-------------------------------------------Active-----------------------------------------------------
                new ToDoSubItemGroup(
                    Guid.NewGuid(),
                    "Group LastCompleted",
                    1,
                    ToDoItemStatus.Planned,
                    string.Empty,
                    false,
                    new ActiveToDoItem(Guid.NewGuid(), "Active"),
                    null
                ),
                new ToDoSubItemPeriodicity(
                    Guid.NewGuid(),
                    "Periodicity LastCompleted",
                    1,
                    ToDoItemStatus.Planned,
                    string.Empty,
                    false,
                    DateTimeOffset.Now,
                    new ActiveToDoItem(Guid.NewGuid(), "Active"),
                    1,
                    1,
                    1,
                    DateTimeOffset.Now,
                    null
                ),
                new ToDoSubItemPlanned(
                    Guid.NewGuid(),
                    "Planned LastCompleted",
                    1u,
                    ToDoItemStatus.Planned,
                    string.Empty,
                    false,
                    new ActiveToDoItem(Guid.NewGuid(), "Active"),
                    DateTimeOffset.Now,
                    1u,
                    1u,
                    1u,
                    false,
                    DateTimeOffset.Now,
                    null
                ),
                new ToDoSubItemValue(
                    Guid.NewGuid(),
                    "Value LastCompleted",
                    false,
                    1,
                    ToDoItemStatus.Planned,
                    string.Empty,
                    1u,
                    1u,
                    1u,
                    false,
                    new ActiveToDoItem(Guid.NewGuid(), "Active"),
                    DateTimeOffset.Now,
                    null
                ),
                new ToDoSubItemPeriodicityOffset(
                    Guid.NewGuid(),
                    "PeriodicityOffset LastCompleted",
                    1u,
                    ToDoItemStatus.Planned,
                    string.Empty,
                    false,
                    DateTimeOffset.Now,
                    new ActiveToDoItem(Guid.NewGuid(), "Active"),
                    1,
                    1,
                    1,
                    DateTimeOffset.Now,
                    null
                ),
            }
        );

        ToDoService = toDoServiceDesign;
        Mapper = ConstDesign.Mapper;

        ToDoSubItemsView = new()
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