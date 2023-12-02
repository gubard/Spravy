using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Avalonia;
using Avalonia.Threading;
using Ninject;
using ReactiveUI;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class LeafToDoItemsViewModel : RoutableViewModelBase, IRefresh
{
    private Guid id;
    private Vector scrollOffset;

    public LeafToDoItemsViewModel() : base("leaf-to-do-items")
    {
        SwitchPaneCommand = CreateCommand(SwitchPane);
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
        ToMultiEditingToDoItemsCommand = CreateInitializedCommand(TaskWork.Create(ToMultiEditingToDoItemsAsync).RunAsync);
    }

    public ICommand InitializedCommand { get; }
    public ICommand SwitchPaneCommand { get; }
    public ICommand ToMultiEditingToDoItemsCommand { get; }

    public Vector ScrollOffset
    {
        get => scrollOffset;
        set => this.RaiseAndSetIfChanged(ref scrollOffset, value);
    }

    [Inject]
    public required IToDoService ToDoService { get; init; }

    [Inject]
    public required IMapper Mapper { get; init; }

    [Inject]
    public required ToDoSubItemsViewModel ToDoSubItemsViewModel { get; init; }

    [Inject]
    public required MainSplitViewModel MainSplitViewModel { get; init; }

    public Guid Id
    {
        get => id;
        set => this.RaiseAndSetIfChanged(ref id, value);
    }

    public async Task RefreshAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var ids = await ToDoService.GetLeafToDoItemIdsAsync(Id, cancellationToken).ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();
        var offset = ScrollOffset;
        await ToDoSubItemsViewModel.UpdateItemsAsync(ids.ToArray(), this, cancellationToken).ConfigureAwait(false);
        await Dispatcher.UIThread.InvokeAsync(() => ScrollOffset = offset);
    }

    private async Task ToMultiEditingToDoItemsAsync(CancellationToken cancellationToken)
    {
        var ids = ToDoSubItemsViewModel.Missed.Concat(ToDoSubItemsViewModel.Planned)
            .Concat(ToDoSubItemsViewModel.ReadyForCompleted)
            .Concat(ToDoSubItemsViewModel.Completed)
            .OrderBy(x => x.OrderIndex)
            .Select(x => x.Id);

        await Navigator.NavigateToAsync<MultiEditingToDoSubItemsViewModel>(
                viewModel => viewModel.Ids.AddRange(ids),
                cancellationToken
            )
            .ConfigureAwait(false);
    }

    private DispatcherOperation SwitchPane()
    {
        return Dispatcher.UIThread.InvokeAsync(() => MainSplitViewModel.IsPaneOpen = !MainSplitViewModel.IsPaneOpen);
    }

    private async Task InitializedAsync(CancellationToken cancellationToken)
    {
        await RefreshAsync(cancellationToken).ConfigureAwait(false);
    }
}