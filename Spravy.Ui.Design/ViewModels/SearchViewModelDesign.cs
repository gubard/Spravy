
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
                null,
                Enumerable.Empty<ToDoShortItem>(),
                Enumerable.Empty<IToDoSubItem>()
            ),
            Completed =
            {
                new ToDoItemNotify()
                {
                    Id = Guid.NewGuid(),
                    Name = Guid.NewGuid().ToString(),
                    Status = ToDoItemStatus.Completed
                },
            },
            Missed =
            {
                new ToDoItemNotify
                {
                    Id = Guid.NewGuid(),
                    Name = Guid.NewGuid().ToString(),
                    Status = ToDoItemStatus.Miss
                },
            },
            Planned =
            {
                new ToDoItemNotify
                {
                    Id = Guid.NewGuid(),
                    Name = Guid.NewGuid().ToString(),
                    Status = ToDoItemStatus.Planned
                },
            },
            ReadyForCompleted =
            {
                new ToDoItemNotify
                {
                    Id = Guid.NewGuid(),
                    Name = Guid.NewGuid().ToString(),
                    Status = ToDoItemStatus.ReadyForComplete
                },
            },
            FavoriteToDoItems =
            {
                new ToDoItemNotify
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