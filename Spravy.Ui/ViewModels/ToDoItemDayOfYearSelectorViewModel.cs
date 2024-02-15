using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Collections;
using Ninject;
using ReactiveUI;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.ToDo.Domain.Models;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class ToDoItemDayOfYearSelectorViewModel : ViewModelBase, IApplySettings
{
    private Guid toDoItemId;

    public ToDoItemDayOfYearSelectorViewModel()
    {
        Items = new(
            Enumerable.Range(1, 12)
                .Select(
                    x => new DayOfYearSelectItem
                    {
                        Month = (byte)x
                    }
                )
        );

        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }

    public AvaloniaList<DayOfYearSelectItem> Items { get; }
    public ICommand InitializedCommand { get; }

    [Inject]
    public required IToDoService ToDoService { get; set; }

    public Guid ToDoItemId
    {
        get => toDoItemId;
        set => this.RaiseAndSetIfChanged(ref toDoItemId, value);
    }

    private async Task InitializedAsync(CancellationToken cancellationToken)
    {
        var annuallyPeriodicity = await ToDoService.GetAnnuallyPeriodicityAsync(ToDoItemId, cancellationToken)
            .ConfigureAwait(false);

        foreach (var item in Items)
        {
            foreach (var day in item.Days)
            {
                if (annuallyPeriodicity.Days.Where(x => x.Month == item.Month).Select(x => x.Day).Contains(day.Day))
                {
                    day.IsSelected = true;
                }
            }
        }
    }

    public Task ApplySettingsAsync(CancellationToken cancellationToken)
    {
        return ToDoService.UpdateToDoItemAnnuallyPeriodicityAsync(
            ToDoItemId,
            new AnnuallyPeriodicity(
                Items.SelectMany(x => x.Days.Where(y => y.IsSelected).Select(y => new DayOfYear(y.Day, x.Month)))
            ),
            cancellationToken
        );
    }
}