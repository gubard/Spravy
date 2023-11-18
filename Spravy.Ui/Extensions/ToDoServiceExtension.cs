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
        var item = await toDoService.GetToDoItemAsync(id).ConfigureAwait(false);

        switch (item)
        {
            case ToDoItemGroup:
                await navigator.NavigateToAsync<ToDoItemGroupViewModel>(vm => vm.Id = item.Id);

                break;
            case ToDoItemPeriodicity:
                await navigator.NavigateToAsync<ToDoItemPeriodicityViewModel>(vm => vm.Id = item.Id);

                break;
            case ToDoItemPlanned:
                await navigator.NavigateToAsync<ToDoItemPlannedViewModel>(vm => vm.Id = item.Id);

                break;
            case ToDoItemValue:
                await navigator.NavigateToAsync<ToDoItemValueViewModel>(vm => vm.Id = item.Id);

                break;
            case ToDoItemPeriodicityOffset:
                await navigator.NavigateToAsync<ToDoItemPeriodicityOffsetViewModel>(vm => vm.Id = item.Id);

                break;
            case ToDoItemCircle:
                await navigator.NavigateToAsync<ToDoItemCircleViewModel>(vm => vm.Id = item.Id);

                break;
            case ToDoItemStep:
                await navigator.NavigateToAsync<ToDoItemStepViewModel>(vm => vm.Id = item.Id);

                break;
            default: throw new ArgumentOutOfRangeException(nameof(item));
        }
    }
}