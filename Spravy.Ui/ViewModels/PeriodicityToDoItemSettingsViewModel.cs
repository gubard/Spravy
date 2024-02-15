using System;
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
using Spravy.Ui.Extensions;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;
using Spravy.Ui.Services;

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
    private IApplySettings? periodicity;

    public PeriodicityToDoItemSettingsViewModel()
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }

    public AvaloniaList<ToDoItemChildrenType> ChildrenTypes { get; } = new(Enum.GetValues<ToDoItemChildrenType>());
    public AvaloniaList<TypeOfPeriodicity> TypeOfPeriodicities { get; } = new(Enum.GetValues<TypeOfPeriodicity>());
    public ICommand InitializedCommand { get; }

    public IApplySettings? Periodicity
    {
        get => periodicity;
        set => this.RaiseAndSetIfChanged(ref periodicity, value);
    }

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

    [Inject]
    public required IKernel Resolve { get; set; }

    private async Task InitializedAsync(CancellationToken cancellationToken)
    {
        this.WhenAnyValue(x => x.TypeOfPeriodicity)
            .Subscribe(
                x => Periodicity = x switch
                {
                    TypeOfPeriodicity.Daily => new EmptyApplySettings(),
                    TypeOfPeriodicity.Weekly => Resolve.Get<ToDoItemDayOfWeekSelectorViewModel>()
                        .Case(y => y.ToDoItemId = Id),
                    TypeOfPeriodicity.Monthly => Resolve.Get<ToDoItemDayOfMonthSelectorViewModel>()
                        .Case(y => y.ToDoItemId = Id),
                    TypeOfPeriodicity.Annually => Resolve.Get<ToDoItemDayOfYearSelectorViewModel>()
                        .Case(y => y.ToDoItemId = Id),
                    _ => throw new ArgumentOutOfRangeException(nameof(x), x, null)
                }
            );

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
            }
        );
    }

    public Task ApplySettingsAsync(CancellationToken cancellationToken)
    {
        return Task.WhenAll(
            ToDoService.UpdateToDoItemChildrenTypeAsync(Id, ChildrenType, cancellationToken),
            ToDoService.UpdateToDoItemDueDateAsync(Id, DueDate, cancellationToken),
            ToDoService.UpdateToDoItemIsRequiredCompleteInDueDateAsync(
                Id,
                IsRequiredCompleteInDueDate,
                cancellationToken
            ),
            ToDoService.UpdateToDoItemTypeOfPeriodicityAsync(Id, TypeOfPeriodicity, cancellationToken),
            Periodicity.ThrowIfNull().ApplySettingsAsync(cancellationToken)
        );
    }
}