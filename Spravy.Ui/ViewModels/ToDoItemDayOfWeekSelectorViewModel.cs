using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Collections;
using Ninject;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.ToDo.Domain.Models;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class ToDoItemDayOfWeekSelectorViewModel : ViewModelBase, IApplySettings
{
    public ToDoItemDayOfWeekSelectorViewModel()
    {
        Items = new(
            Enum.GetValues<DayOfWeek>()
                .Select(
                    x => new DayOfWeekSelectItem
                    {
                        DayOfWeek = x,
                    }
                )
        );

        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }

    public AvaloniaList<DayOfWeekSelectItem> Items { get; }
    public ICommand InitializedCommand { get; }

    [Inject]
    public required IToDoService ToDoService { get; set; }

    [Reactive]
    public Guid ToDoItemId { get; set; }

    private async Task InitializedAsync(CancellationToken cancellationToken)
    {
        var weeklyPeriodicity = await ToDoService.GetWeeklyPeriodicityAsync(ToDoItemId, cancellationToken)
            .ConfigureAwait(false);

        foreach (var item in Items)
        {
            if (weeklyPeriodicity.Days.Contains(item.DayOfWeek))
            {
                item.IsSelected = true;
            }
        }
    }

    public Task ApplySettingsAsync(CancellationToken cancellationToken)
    {
        return ToDoService.UpdateToDoItemWeeklyPeriodicityAsync(
            ToDoItemId,
            new WeeklyPeriodicity(Items.Where(x => x.IsSelected).Select(x => x.DayOfWeek)),
            cancellationToken
        );
    }
}