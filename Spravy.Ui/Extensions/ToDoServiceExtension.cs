using System;
using System.Threading.Tasks;
using ExtensionFramework.ReactiveUI.Interfaces;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
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
            default: throw new ArgumentOutOfRangeException(nameof(item));
        }
    }
}