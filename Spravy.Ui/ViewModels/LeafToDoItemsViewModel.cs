using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Ninject;
using ProtoBuf;
using ReactiveUI;
using Spravy.Domain.Extensions;
using Spravy.Domain.Helpers;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.Ui.Extensions;
using Spravy.Ui.Features.ToDo.Enums;
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
    public override string ViewId => $"{TypeCache<LeafToDoItemsViewModel>.Type.Name}:{Id}";

    public Vector ScrollOffset
    {
        get => scrollOffset;
        set => this.RaiseAndSetIfChanged(ref scrollOffset, value);
    }

    [Inject]
    public required IObjectStorage ObjectStorage { get; init; }

    [Inject]
    public required PageHeaderViewModel PageHeaderViewModel { get; init; }

    [Inject]
    public required IToDoService ToDoService { get; init; }

    [Inject]
    public required ToDoSubItemsViewModel ToDoSubItemsViewModel { get; init; }

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
                    CommandStorage.MultiCompleteToDoItemsItem.WithParam(
                        ToDoSubItemsViewModel.List.MultiToDoItems.GroupByNone.Items.Items
                    )
                );
                PageHeaderViewModel.Commands.Add(
                    CommandStorage.MultiSetTypeToDoItemsItem.WithParam(
                        ToDoSubItemsViewModel.List.MultiToDoItems.GroupByNone.Items.Items
                    )
                );
                PageHeaderViewModel.Commands.Add(
                    CommandStorage.MultiSetRootToDoItemsItem.WithParam(
                        ToDoSubItemsViewModel.List.MultiToDoItems.GroupByNone.Items.Items
                    )
                );
            }
        );

        var setting = await ObjectStorage.GetObjectOrDefaultAsync<LeafToDoItemsViewModelSetting>(ViewId)
            .ConfigureAwait(false);

        await SetStateAsync(setting).ConfigureAwait(false);
        await RefreshAsync(cancellationToken).ConfigureAwait(false);
    }

    public override void Stop()
    {
        refreshWork.Cancel();
    }

    public override Task SaveStateAsync()
    {
        return ObjectStorage.SaveObjectAsync(
            ViewId,
            new LeafToDoItemsViewModelSetting(this)
        );
    }

    public override async Task SetStateAsync(object setting)
    {
        var s = setting.ThrowIfIsNotCast<LeafToDoItemsViewModelSetting>();

        await this.InvokeUIBackgroundAsync(
            () =>
            {
                ToDoSubItemsViewModel.List.GroupBy = s.GroupBy;
                ToDoSubItemsViewModel.List.IsMulti = s.IsMulti;
            }
        );
    }

    [ProtoContract]
    class LeafToDoItemsViewModelSetting
    {
        public LeafToDoItemsViewModelSetting(LeafToDoItemsViewModel viewModel)
        {
            GroupBy = viewModel.ToDoSubItemsViewModel.List.GroupBy;
            IsMulti = viewModel.ToDoSubItemsViewModel.List.IsMulti;
        }

        public LeafToDoItemsViewModelSetting()
        {
        }

        [ProtoMember(1)]
        public GroupBy GroupBy { get; set; } = GroupBy.ByStatus;

        [ProtoMember(2)]
        public bool IsMulti { get; set; }
    }
}