using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Ninject;
using ReactiveUI;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.Ui.Extensions;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class PlannedToDoItemSettingsViewModel : ViewModelBase, IToDoChildrenTypeProperty, IToDoDueDateProperty, IIsRequiredCompleteInDueDateProperty
{
    private ToDoItemChildrenType childrenType;
    private Guid id;
    private DateOnly dueDate;
    private bool isRequiredCompleteInDueDate;

    public PlannedToDoItemSettingsViewModel()
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
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
        var setting = await ToDoService.GetPlannedToDoItemSettingsAsync(Id, cancellationToken).ConfigureAwait(false);

        await this.InvokeUIBackgroundAsync(
            () =>
            {
                ChildrenType = setting.ChildrenType;
                DueDate = setting.DueDate;
            }
        );
    }
}