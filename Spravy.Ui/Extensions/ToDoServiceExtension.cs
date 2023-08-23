using System;
using System.Threading.Tasks;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.ToDo.Domain.Models;
using Spravy.Ui.Interfaces;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Extensions;

public static class ToDoServiceExtension
{
    public static async Task NavigateToToDoItemViewModel(this IToDoService toDoService, Guid id, INavigator navigator)
    {
        var item = await toDoService.GetToDoItemAsync(id);

        switch (item)
        {
            case ToDoItemGroup toDoItemGroup:
                navigator.NavigateTo<ToDoItemGroupViewModel>(vm => vm.Id = toDoItemGroup.Id);

                break;
            case ToDoItemPeriodicity toDoItemPeriodicity:
                navigator.NavigateTo<ToDoItemPeriodicityViewModel>(vm => vm.Id = toDoItemPeriodicity.Id);

                break;
            case ToDoItemPlanned toDoItemPlanned:
                navigator.NavigateTo<ToDoItemPlannedViewModel>(vm => vm.Id = toDoItemPlanned.Id);

                break;
            case ToDoItemValue toDoItemValue:
                navigator.NavigateTo<ToDoItemValueViewModel>(vm => vm.Id = toDoItemValue.Id);

                break;
            case ToDoItemPeriodicityOffset toDoItemPeriodicityOffset:
                navigator.NavigateTo<ToDoItemPeriodicityOffsetViewModel>(vm => vm.Id = toDoItemPeriodicityOffset.Id);

                break;
            default: throw new ArgumentOutOfRangeException(nameof(item));
        }
    }
}