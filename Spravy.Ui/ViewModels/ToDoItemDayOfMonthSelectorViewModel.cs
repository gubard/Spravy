using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Collections;
using Ninject;
using ReactiveUI.Fody.Helpers;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.ToDo.Domain.Models;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class ToDoItemDayOfMonthSelectorViewModel : ViewModelBase, IApplySettings
{
    public ToDoItemDayOfMonthSelectorViewModel()
    {
        Items = new(
            Enumerable.Range(1, 31)
                .Select(
                    x => new DayOfMonthSelectItem
                    {
                        Day = (byte)x
                    }
                )
        );

        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }

    public AvaloniaList<DayOfMonthSelectItem> Items { get; }
    public ICommand InitializedCommand { get; }

    [Inject]
    public required IToDoService ToDoService { get; set; }

    [Reactive]
    public Guid ToDoItemId { get; set; }

    private async Task InitializedAsync(CancellationToken cancellationToken)
    {
        var monthlyPeriodicity = await ToDoService.GetMonthlyPeriodicityAsync(ToDoItemId, cancellationToken)
            .ConfigureAwait(false);

        foreach (var item in Items)
        {
            if (monthlyPeriodicity.Days.Contains(item.Day))
            {
                item.IsSelected = true;
            }
        }
    }

    public Task ApplySettingsAsync(CancellationToken cancellationToken)
    {
        return ToDoService.UpdateToDoItemMonthlyPeriodicityAsync(
            ToDoItemId,
            new MonthlyPeriodicity(Items.Where(x => x.IsSelected).Select(x => x.Day)),
            cancellationToken
        );
    }
}