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

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        return ToDoService.GetWeeklyPeriodicityAsync(ToDoItemId, cancellationToken)
            .IfSuccessAsync(
                weeklyPeriodicity => Result.AwaitableFalse.IfSuccessAllAsync(
                    cancellationToken,
                    Items.Where(x => weeklyPeriodicity.Days.Contains(x.DayOfWeek))
                        .Select<DayOfWeekSelectItem, Func<ConfiguredValueTaskAwaitable<Result>>>(
                            x =>
                            {
                                var y = x;

                                return () => this.InvokeUIBackgroundAsync(() => y.IsSelected = true);
                            }
                        )
                        .ToArray()
                ),
                cancellationToken
            );
    }

    public ConfiguredValueTaskAwaitable<Result> ApplySettingsAsync(CancellationToken cancellationToken)
    {
        return ToDoService.UpdateToDoItemWeeklyPeriodicityAsync(
            ToDoItemId,
            new WeeklyPeriodicity(Items.Where(x => x.IsSelected).Select(x => x.DayOfWeek)),
            cancellationToken
        );
    }
}