using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Collections;
using Ninject;
using ReactiveUI;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.Ui.Extensions;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class PeriodicityToDoItemSettingsViewModel : ViewModelBase,
    IToDoChildrenTypeProperty,
    IToDoDueDateProperty,
    IToDoTypeOfPeriodicityProperty,
    IIsRequiredCompleteInDueDateProperty,
    IApplySettings
{
    private ToDoItemChildrenType childrenType;
    private Guid id;
    private DateOnly dueDate;
    private TypeOfPeriodicity typeOfPeriodicity;
    private bool isRequiredCompleteInDueDate;

    public PeriodicityToDoItemSettingsViewModel()
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }

    public AvaloniaList<string> Values { get; } = new();

    public bool IsRequiredCompleteInDueDate
    {
        get => isRequiredCompleteInDueDate;
        set => this.RaiseAndSetIfChanged(ref isRequiredCompleteInDueDate, value);
    }

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

    public ICommand InitializedCommand { get; }

    private async Task InitializedAsync(CancellationToken cancellationToken)
    {
        await RefreshAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task RefreshAsync(CancellationToken cancellationToken)
    {
        var setting = await ToDoService.GetPeriodicityToDoItemSettingsAsync(Id, cancellationToken)
            .ConfigureAwait(false);

        await this.InvokeUIBackgroundAsync(
            () =>
            {
                ChildrenType = setting.ChildrenType;
                DueDate = setting.DueDate;
                TypeOfPeriodicity = setting.TypeOfPeriodicity;
                IsRequiredCompleteInDueDate = setting.IsRequiredCompleteInDueDate;
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

    public async Task ApplySettingsAsync(CancellationToken cancellationToken)
    {
        await ToDoService.UpdateToDoItemChildrenTypeAsync(Id, ChildrenType, cancellationToken).ConfigureAwait(false);
        await ToDoService.UpdateToDoItemDueDateAsync(Id, DueDate, cancellationToken).ConfigureAwait(false);

        await ToDoService
            .UpdateToDoItemIsRequiredCompleteInDueDateAsync(Id, IsRequiredCompleteInDueDate, cancellationToken)
            .ConfigureAwait(false);

        await ToDoService
            .UpdateToDoItemTypeOfPeriodicityAsync(Id, TypeOfPeriodicity, cancellationToken)
            .ConfigureAwait(false);
    }
}