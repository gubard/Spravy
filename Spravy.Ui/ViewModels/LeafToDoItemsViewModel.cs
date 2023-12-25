using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Avalonia;
using Material.Icons;
using Ninject;
using ReactiveUI;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.Ui.Extensions;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;
using Spravy.Ui.Services;

namespace Spravy.Ui.ViewModels;

public class LeafToDoItemsViewModel : NavigatableViewModelBase, IRefresh
{
    private Guid id;
    private Vector scrollOffset;
    private readonly TaskWork refreshWork;

    public LeafToDoItemsViewModel() : base(true)
    {
        refreshWork = TaskWork.Create(RefreshCoreAsync);
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }

    public ICommand InitializedCommand { get; }

    public Vector ScrollOffset
    {
        get => scrollOffset;
        set => this.RaiseAndSetIfChanged(ref scrollOffset, value);
    }

    [Inject]
    public required PageHeaderViewModel PageHeaderViewModel { get; init; }

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

    public Task RefreshAsync(CancellationToken cancellationToken)
    {
        return refreshWork.RunAsync();
    }

    private async Task RefreshCoreAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var ids = await ToDoService.GetLeafToDoItemIdsAsync(Id, cancellationToken).ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();
        var offset = ScrollOffset;
        await ToDoSubItemsViewModel.UpdateItemsAsync(ids.ToArray(), this, cancellationToken).ConfigureAwait(false);
        await this.InvokeUIAsync(() => ScrollOffset = offset);
    }

    private async Task InitializedAsync(CancellationToken cancellationToken)
    {
        await this.InvokeUIAsync(
            () =>
            {
                PageHeaderViewModel.Commands.Add(
                    new CommandItem(
                        MaterialIconKind.CheckAll,
                        CommandStorage.MultiCompleteToDoItemsCommand,
                        string.Empty,
                        ToDoSubItemsViewModel.List.MultiToDoItems.GroupByNone.Items.Items
                    )
                );
                PageHeaderViewModel.Commands.Add(
                    new CommandItem(
                        MaterialIconKind.Switch,
                        ToDoSubItemsViewModel.MultiChangeTypeCommand,
                        string.Empty,
                        null
                    )
                );
                PageHeaderViewModel.Commands.Add(
                    new CommandItem(
                        MaterialIconKind.SwapHorizontal,
                        ToDoSubItemsViewModel.MultiSetParentItemCommand,
                        string.Empty,
                        null
                    )
                );
            }
        );
        await RefreshAsync(cancellationToken).ConfigureAwait(false);
    }

    public override void Stop()
    {
        refreshWork.Cancel();
    }
}