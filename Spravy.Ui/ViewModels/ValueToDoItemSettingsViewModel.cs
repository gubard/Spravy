using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
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

public class ValueToDoItemSettingsViewModel : ViewModelBase
{
    private ToDoItemChildrenType childrenType;
    private Guid id;

    public ValueToDoItemSettingsViewModel()
    {
        ChangeChildrenTypeCommand = CreateCommandFromTask(TaskWork.Create(ChangeChildrenTypeAsync).RunAsync);
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
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

    [Inject]
    public required IToDoService ToDoService { get; set; }

    public IRefresh? Refresh { get; set; }

    public ICommand ChangeChildrenTypeCommand { get; }
    public ICommand InitializedCommand { get; }


    private async Task InitializedAsync(CancellationToken cancellationToken)
    {
        await RefreshAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task RefreshAsync(CancellationToken cancellationToken)
    {
        var setting = await ToDoService.GetValueToDoItemSettingsAsync(Id, cancellationToken).ConfigureAwait(false);
        await this.InvokeUIBackgroundAsync(() => ChildrenType = setting.ChildrenType);
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