using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Threading;
using Ninject;
using ReactiveUI;
using Spravy.Domain.Extensions;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.Ui.Extensions;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class PlannedToDoItemSettingsViewModel : ViewModelBase
{
    private ToDoItemChildrenType childrenType;
    private Guid id;
    private DateOnly dueDate;

    public PlannedToDoItemSettingsViewModel()
    {
        ChangeChildrenTypeCommand = CreateCommandFromTask(TaskWork.Create(ChangeChildrenTypeAsync).RunAsync);
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
        ChangeDueDateCommand = CreateInitializedCommand(TaskWork.Create(ChangeDueDateAsync).RunAsync);
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

    public IRefresh? Refresh { get; set; }

    public ICommand ChangeChildrenTypeCommand { get; }
    public ICommand InitializedCommand { get; }
    public ICommand ChangeDueDateCommand { get; }

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
        var setting = await ToDoService.GetPlannedToDoItemSettingsAsync(Id, cancellationToken).ConfigureAwait(false);

        await Dispatcher.UIThread.InvokeAsync(
            () =>
            {
                ChildrenType = setting.ChildrenType;
                DueDate = setting.DueDate;
            }
        );
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