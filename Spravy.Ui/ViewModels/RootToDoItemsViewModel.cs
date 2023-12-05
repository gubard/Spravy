using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Avalonia.Threading;
using Ninject;
using Spravy.Domain.Helpers;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.ToDo.Domain.Models;
using Spravy.Ui.Extensions;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class RootToDoItemsViewModel : NavigatableViewModelBase, IToDoItemOrderChanger
{
    private readonly TaskWork refreshWork;

    public RootToDoItemsViewModel() : base(true)
    {
        refreshWork = TaskWork.Create(RefreshCoreAsync);
        InitializedCommand = CreateInitializedCommand(refreshWork.RunAsync);
        AddToDoItemCommand = CreateCommandFromTask(TaskWork.Create(AddToDoItemAsync).RunAsync);
        SwitchPaneCommand = CreateCommand(SwitchPane);
    }

    public ICommand InitializedCommand { get; }
    public ICommand AddToDoItemCommand { get; }
    public ICommand SwitchPaneCommand { get; }

    [Inject]
    public required ToDoSubItemsViewModel ToDoSubItemsViewModel { get; init; }

    [Inject]
    public required IToDoService ToDoService { get; init; }

    [Inject]
    public required IMapper Mapper { get; init; }

    [Inject]
    public required MainSplitViewModel MainSplitViewModel { get; init; }

    private DispatcherOperation SwitchPane()
    {
        return this.InvokeUIAsync(() => MainSplitViewModel.IsPaneOpen = !MainSplitViewModel.IsPaneOpen);
    }

    public Task RefreshAsync(CancellationToken cancellationToken)
    {
        return refreshWork.RunAsync();
    }

    private async Task RefreshCoreAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var ids = await ToDoService.GetRootToDoItemIdsAsync(cancellationToken).ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();
        await ToDoSubItemsViewModel.UpdateItemsAsync(ids.ToArray(), this, cancellationToken).ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();
    }


    private async Task AddToDoItemAsync(CancellationToken cancellationToken)
    {
        await DialogViewer.ShowConfirmContentDialogAsync(
                async view =>
                {
                    await DialogViewer.CloseContentDialogAsync(cancellationToken).ConfigureAwait(false);
                    cancellationToken.ThrowIfCancellationRequested();
                    var options = Mapper.Map<AddRootToDoItemOptions>(view);
                    cancellationToken.ThrowIfCancellationRequested();
                    await ToDoService.AddRootToDoItemAsync(options, cancellationToken).ConfigureAwait(false);
                    cancellationToken.ThrowIfCancellationRequested();
                    await RefreshAsync(cancellationToken).ConfigureAwait(false);
                },
                async _ => await DialogViewer.CloseContentDialogAsync(cancellationToken).ConfigureAwait(false),
                ActionHelper<AddRootToDoItemViewModel>.Empty,
                cancellationToken
            )
            .ConfigureAwait(false);
    }

    public override void Stop()
    {
        refreshWork.Cancel();
    }
}