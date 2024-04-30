using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Input;
using Avalonia.Collections;
using Ninject;
using ReactiveUI.Fody.Helpers;
using Spravy.Domain.Extensions;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.Ui.Extensions;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;
using Spravy.Ui.Services;

namespace Spravy.Ui.ViewModels;

public class ToDoItemDayOfYearSelectorViewModel : ViewModelBase, IApplySettings
{
    public ToDoItemDayOfYearSelectorViewModel()
    {
        Items = new(Enumerable.Range(1, 12)
           .Select(x => new DayOfYearSelectItem
            {
                Month = (byte)x,
            }));

        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }

    public AvaloniaList<DayOfYearSelectItem> Items { get; }
    public ICommand InitializedCommand { get; }

    [Inject]
    public required IToDoService ToDoService { get; set; }

    [Reactive]
    public Guid ToDoItemId { get; set; }

    public ConfiguredValueTaskAwaitable<Result> ApplySettingsAsync(CancellationToken cancellationToken)
    {
        return ToDoService.UpdateToDoItemAnnuallyPeriodicityAsync(ToDoItemId,
            new(Items.SelectMany(x => x.Days.Where(y => y.IsSelected).Select(y => new DayOfYear(y.Day, x.Month)))),
            cancellationToken);
    }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        return ToDoService.GetAnnuallyPeriodicityAsync(ToDoItemId, cancellationToken)
           .IfSuccessAsync(annuallyPeriodicity =>
            {
                var items = new List<Func<ConfiguredValueTaskAwaitable<Result>>>();

                foreach (var item in Items)
                {
                    foreach (var day in item.Days)
                    {
                        if (annuallyPeriodicity.Days
                           .Where(x => x.Month == item.Month)
                           .Select(x => x.Day)
                           .Contains(day.Day))
                        {
                            var d = day;
                            items.Add(() => this.InvokeUIBackgroundAsync(() => d.IsSelected = true));
                        }
                    }
                }

                return Result.AwaitableFalse.IfSuccessAllAsync(cancellationToken, items.ToArray());
            }, cancellationToken);
    }
}