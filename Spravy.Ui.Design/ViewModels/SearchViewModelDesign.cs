
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.ToDo.Domain.Models;
using Spravy.Ui.Design.Helpers;
using Spravy.Ui.Design.Services;
using Spravy.Ui.Models;
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
                new ToDoItemGroup(),
                Enumerable.Empty<ToDoShortItem>(),
                Enumerable.Empty<IToDoSubItem>()
            ),
            Completed =
            {
                new ToDoSubItemGroupNotify
                {
                    Id = Guid.NewGuid(),
                    Name = Guid.NewGuid().ToString(),
                    Status = ToDoItemStatus.Completed
                },
            },
            Missed =
            {
                new ToDoSubItemGroupNotify
                {
                    Id = Guid.NewGuid(),
                    Name = Guid.NewGuid().ToString(),
                    Status = ToDoItemStatus.Miss
                },
            },
            Planned =
            {
                new ToDoSubItemGroupNotify
                {
                    Id = Guid.NewGuid(),
                    Name = Guid.NewGuid().ToString(),
                    Status = ToDoItemStatus.Planned
                },
            },
            ReadyForCompleted =
            {
                new ToDoSubItemGroupNotify
                {
                    Id = Guid.NewGuid(),
                    Name = Guid.NewGuid().ToString(),
                    Status = ToDoItemStatus.ReadyForComplete
                },
            },
            FavoriteToDoItems =
            {
                new ToDoSubItemGroupNotify
                {
                    Id = Guid.NewGuid(),
                    Name = Guid.NewGuid().ToString(),
                    Status = ToDoItemStatus.ReadyForComplete,
                    IsFavorite = true
                },
            }
        };
    }
}