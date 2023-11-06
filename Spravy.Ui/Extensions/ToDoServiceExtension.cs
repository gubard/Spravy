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
            case ToDoItemGroup:
                navigator.NavigateTo<ToDoItemGroupViewModel>(vm => vm.Id = item.Id);

                break;
            case ToDoItemPeriodicity:
                navigator.NavigateTo<ToDoItemPeriodicityViewModel>(vm => vm.Id = item.Id);

                break;
            case ToDoItemPlanned:
                navigator.NavigateTo<ToDoItemPlannedViewModel>(vm => vm.Id = item.Id);

                break;
            case ToDoItemValue:
                navigator.NavigateTo<ToDoItemValueViewModel>(vm => vm.Id = item.Id);

                break;
            case ToDoItemPeriodicityOffset:
                navigator.NavigateTo<ToDoItemPeriodicityOffsetViewModel>(vm => vm.Id = item.Id);

                break;
            case ToDoItemCircle:
                navigator.NavigateTo<ToDoItemCircleViewModel>(vm => vm.Id = item.Id);

                break;
            case ToDoItemStep:
                navigator.NavigateTo<ToDoItemStepViewModel>(vm => vm.Id = item.Id);

                break;
            default: throw new ArgumentOutOfRangeException(nameof(item));
        }
    }
}