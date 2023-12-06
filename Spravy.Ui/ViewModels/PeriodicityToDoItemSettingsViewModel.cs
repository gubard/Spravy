using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Collections;
using Ninject;
using ReactiveUI;
using Spravy.Domain.Extensions;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.ToDo.Domain.Models;
using Spravy.Ui.Extensions;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class PeriodicityToDoItemSettingsViewModel : ViewModelBase
{
    private ToDoItemChildrenType childrenType;
    private Guid id;
    private DateOnly dueDate;
    private TypeOfPeriodicity typeOfPeriodicity;

    public PeriodicityToDoItemSettingsViewModel()
    {
        ChangeChildrenTypeCommand = CreateCommandFromTask(TaskWork.Create(ChangeChildrenTypeAsync).RunAsync);
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
        ChangeDueDateCommand = CreateInitializedCommand(TaskWork.Create(ChangeDueDateAsync).RunAsync);
        ChangeTypeOfPeriodicityCommand =
            CreateInitializedCommand(TaskWork.Create(ChangeTypeOfPeriodicityAsync).RunAsync);
        ChangePeriodicityCommand = CreateInitializedCommand(TaskWork.Create(ChangePeriodicityAsync).RunAsync);
    }

    public AvaloniaList<string> Values { get; } = new();

    public Guid Id
    {
        get => id;
        set => this.RaiseAndSetIfChanged(ref id, value);
    }

    public ToDoItemChildrenType ChildrenType
    {
        get => childrenType;
        set => this.RaiseAndSetIfChanged(ref childrenType, value);
    }

    public TypeOfPeriodicity TypeOfPeriodicity
    {
        get => typeOfPeriodicity;
        set => this.RaiseAndSetIfChanged(ref typeOfPeriodicity, value);
    }

    public DateOnly DueDate
    {
        get => dueDate;
        set => this.RaiseAndSetIfChanged(ref dueDate, value);
    }

    [Inject]
    public required IToDoService ToDoService { get; set; }

    public IRefresh? Refresh { get; set; }

    public ICommand ChangeChildrenTypeCommand { get; }
    public ICommand ChangePeriodicityCommand { get; }
    public ICommand InitializedCommand { get; }
    public ICommand ChangeDueDateCommand { get; }
    public ICommand ChangeTypeOfPeriodicityCommand { get; }

    private async Task ChangePeriodicityAsync(CancellationToken cancellationToken)
    {
        switch (TypeOfPeriodicity)
        {
            case TypeOfPeriodicity.Weekly:
            {
                var periodicity = await ToDoService.GetWeeklyPeriodicityAsync(Id, cancellationToken)
                    .ConfigureAwait(false);

                await DialogViewer.ShowDayOfWeekSelectorInputDialogAsync(
                        async days =>
                        {
                            await DialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);
                            await ToDoService.UpdateToDoItemWeeklyPeriodicityAsync(
                                    Id,
                                    new WeeklyPeriodicity(days),
                                    cancellationToken
                                )
                                .ConfigureAwait(false);
                            await RefreshAsync(cancellationToken).ConfigureAwait(false);
                            await Refresh.ThrowIfNull().RefreshAsync(cancellationToken).ConfigureAwait(false);
                        },
                        viewModel =>
                        {
                            foreach (var item in viewModel.Items)
                            {
                                if (periodicity.Days.Contains(item.DayOfWeek))
                                {
                                    item.IsSelected = true;
                                }
                            }
                        },
                        cancellationToken
                    )
                    .ConfigureAwait(false);

                break;
            }
            case TypeOfPeriodicity.Monthly:
            {
                var periodicity = await ToDoService.GetMonthlyPeriodicityAsync(Id, cancellationToken)
                    .ConfigureAwait(false);

                await DialogViewer.ShowDayOfMonthSelectorInputDialogAsync(
                        async days =>
                        {
                            await DialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);
                            await ToDoService.UpdateToDoItemMonthlyPeriodicityAsync(
                                    Id,
                                    new MonthlyPeriodicity(days),
                                    cancellationToken
                                )
                                .ConfigureAwait(false);
                            await RefreshAsync(cancellationToken).ConfigureAwait(false);
                            await Refresh.ThrowIfNull().RefreshAsync(cancellationToken).ConfigureAwait(false);
                        },
                        viewModel =>
                        {
                            foreach (var item in viewModel.Items)
                            {
                                if (periodicity.Days.Contains(item.Day))
                                {
                                    item.IsSelected = true;
                                }
                            }
                        },
                        cancellationToken
                    )
                    .ConfigureAwait(false);

                break;
            }
            case TypeOfPeriodicity.Annually:
            {
                var periodicity = await ToDoService.GetAnnuallyPeriodicityAsync(Id, cancellationToken)
                    .ConfigureAwait(false);

                await DialogViewer.ShowDayOfYearSelectorInputDialogAsync(
                        async days =>
                        {
                            await DialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);
                            await ToDoService.UpdateToDoItemAnnuallyPeriodicityAsync(
                                    Id,
                                    new AnnuallyPeriodicity(days),
                                    cancellationToken
                                )
                                .ConfigureAwait(false);
                            await RefreshAsync(cancellationToken).ConfigureAwait(false);
                            await Refresh.ThrowIfNull().RefreshAsync(cancellationToken).ConfigureAwait(false);
                        },
                        viewModel =>
                        {
                            foreach (var month in viewModel.Items)
                            {
                                foreach (var day in month.Days.Items)
                                {
                                    if (periodicity.Days.Any(x => x.Month == month.Month && x.Day == day.Day))
                                    {
                                        day.IsSelected = true;
                                    }
                                }
                            }
                        },
                        cancellationToken
                    )
                    .ConfigureAwait(false);

                break;
            }
            case TypeOfPeriodicity.Daily:
                throw new ArgumentOutOfRangeException();
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task ChangeTypeOfPeriodicityAsync(CancellationToken cancellationToken)
    {
        await DialogViewer.ShowItemSelectorDialogAsync<TypeOfPeriodicity>(
                async value =>
                {
                    await DialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);
                    await ToDoService.UpdateToDoItemTypeOfPeriodicityAsync(Id, value, cancellationToken)
                        .ConfigureAwait(false);
                    await RefreshAsync(cancellationToken).ConfigureAwait(false);
                    await Refresh.ThrowIfNull().RefreshAsync(cancellationToken).ConfigureAwait(false);
                },
                calendar =>
                {
                    calendar.Items.AddRange(Enum.GetValues<TypeOfPeriodicity>().OfType<object>());
                    calendar.SelectedItem = TypeOfPeriodicity;
                },
                cancellationToken
            )
            .ConfigureAwait(false);
    }

    private async Task ChangeDueDateAsync(CancellationToken cancellationToken)
    {
        await DialogViewer.ShowDateConfirmDialogAsync(
                async value =>
                {
                    await DialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);
                    await ToDoService.UpdateToDoItemDueDateAsync(Id, value.ToDateOnly(), cancellationToken)
                        .ConfigureAwait(false);
                    await RefreshAsync(cancellationToken).ConfigureAwait(false);
                    await Refresh.ThrowIfNull().RefreshAsync(cancellationToken).ConfigureAwait(false);
                },
                calendar => calendar.SelectedDate = DueDate.ToDateTime(),
                cancellationToken
            )
            .ConfigureAwait(false);
    }

    private async Task InitializedAsync(CancellationToken cancellationToken)
    {
        await RefreshAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task RefreshAsync(CancellationToken cancellationToken)
    {
        var setting = await ToDoService.GetPeriodicityToDoItemSettingsAsync(Id, cancellationToken)
            .ConfigureAwait(false);

        await this.InvokeUIBackgroundAsync(
            () =>
            {
                ChildrenType = setting.ChildrenType;
                DueDate = setting.DueDate;
                TypeOfPeriodicity = setting.TypeOfPeriodicity;
                Values.Clear();
            }
        );

        switch (TypeOfPeriodicity)
        {
            case TypeOfPeriodicity.Daily:
                break;
            case TypeOfPeriodicity.Weekly:
            {
                var periodicity = await ToDoService.GetWeeklyPeriodicityAsync(Id, cancellationToken)
                    .ConfigureAwait(false);

                await this.InvokeUIBackgroundAsync(
                    () => Values.AddRange(periodicity.Days.Select(x => x.ToString()))
                );
                break;
            }
            case TypeOfPeriodicity.Monthly:
            {
                var periodicity = await ToDoService.GetMonthlyPeriodicityAsync(Id, cancellationToken)
                    .ConfigureAwait(false);

                await this.InvokeUIBackgroundAsync(
                    () => Values.AddRange(periodicity.Days.Select(x => x.ToString()))
                );

                break;
            }
            case TypeOfPeriodicity.Annually:
            {
                var periodicity = await ToDoService.GetAnnuallyPeriodicityAsync(Id, cancellationToken)
                    .ConfigureAwait(false);

                await this.InvokeUIBackgroundAsync(
                    () => Values.AddRange(periodicity.Days.Select(x => $"{x.Day}.{x.Month}"))
                );

                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task ChangeChildrenTypeAsync(CancellationToken cancellationToken)
    {
        await DialogViewer.ShowItemSelectorDialogAsync<ToDoItemChildrenType>(
                async item =>
                {
                    await DialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);
                    await ToDoService.UpdateToDoItemChildrenTypeAsync(Id, item, cancellationToken)
                        .ConfigureAwait(false);
                    await RefreshAsync(cancellationToken).ConfigureAwait(false);
                    await Refresh.ThrowIfNull().RefreshAsync(cancellationToken).ConfigureAwait(false);
                },
                viewModel =>
                {
                    viewModel.Items.AddRange(Enum.GetValues<ToDoItemChildrenType>().OfType<object>());
                    viewModel.SelectedItem = ChildrenType;
                },
                cancellationToken
            )
            .ConfigureAwait(false);
    }
}