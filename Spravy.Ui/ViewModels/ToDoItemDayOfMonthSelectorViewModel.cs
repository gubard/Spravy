using System;
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
using Spravy.ToDo.Domain.Models;
using Spravy.Ui.Extensions;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;
using Spravy.Ui.Services;

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

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        return ToDoService.GetMonthlyPeriodicityAsync(ToDoItemId, cancellationToken)
            .IfSuccessAsync(
                monthlyPeriodicity => Result.AwaitableFalse.IfSuccessAllAsync(
                        Items.Where(x => monthlyPeriodicity.Days.Contains(x.Day))
                            .Select<DayOfMonthSelectItem, Func<ConfiguredValueTaskAwaitable<Result>>>(
                                x =>
                                {
                                    var y = x;

                                    return () => this.InvokeUIBackgroundAsync(() => y.IsSelected = true);
                                }
                            )
                            .ToArray()
                    )
            );
    }

    public ConfiguredValueTaskAwaitable<Result> ApplySettingsAsync(CancellationToken cancellationToken)
    {
        return ToDoService.UpdateToDoItemMonthlyPeriodicityAsync(
            ToDoItemId,
            new MonthlyPeriodicity(Items.Where(x => x.IsSelected).Select(x => x.Day)),
            cancellationToken
        );
    }
}