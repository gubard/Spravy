using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Ninject;
using ProtoBuf;
using ReactiveUI.Fody.Helpers;
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
    private readonly TaskWork refreshWork;
    private readonly PageHeaderViewModel pageHeaderViewModel;

    public LeafToDoItemsViewModel() : base(true)
    {
        refreshWork = TaskWork.Create(RefreshCoreAsync);
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }

    public ICommand InitializedCommand { get; }
    public override string ViewId => $"{TypeCache<LeafToDoItemsViewModel>.Type.Name}:{Id}";

    [Inject]
    public required IObjectStorage ObjectStorage { get; init; }

    [Inject]
    public required PageHeaderViewModel PageHeaderViewModel
    {
        get => pageHeaderViewModel;
        [MemberNotNull(nameof(pageHeaderViewModel))]
        init
        {
            pageHeaderViewModel = value;
            pageHeaderViewModel.LeftCommand = CommandStorage.NavigateToCurrentToDoItemItem;
        }
    }

    [Inject]
    public required IToDoService ToDoService { get; init; }

    [Inject]
    public required ToDoSubItemsViewModel ToDoSubItemsViewModel { get; init; }

    [Reactive]
    public Guid Id { get; set; }

    public async ValueTask<Result> RefreshAsync(CancellationToken cancellationToken)
    {
        await refreshWork.RunAsync();

        return Result.Success;
    }

    private ValueTask<Result> RefreshCoreAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return ToDoService.GetLeafToDoItemIdsAsync(Id, cancellationToken)
            .ConfigureAwait(false)
            .IfSuccessAsync(
                ids => ToDoSubItemsViewModel.UpdateItemsAsync(ids.ToArray(), this, true, cancellationToken)
                    .ConfigureAwait(false)
            );
    }

    private ValueTask<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        return PageHeaderViewModel.SetMultiCommands(ToDoSubItemsViewModel)
            .ConfigureAwait(false)
            .IfSuccessAsync(
                () => ObjectStorage.GetObjectOrDefaultAsync<LeafToDoItemsViewModelSetting>(ViewId)
                    .ConfigureAwait(false)
                    .IfSuccessAsync(s => SetStateAsync(s).ConfigureAwait(false))
                    .ConfigureAwait(false)
            )
            .ConfigureAwait(false)
            .IfSuccessAsync(() => RefreshAsync(cancellationToken).ConfigureAwait(false));
    }

    public override Result Stop()
    {
        refreshWork.Cancel();

        return Result.Success;
    }

    public override ValueTask<Result> SaveStateAsync()
    {
        return ObjectStorage.SaveObjectAsync(
            ViewId,
            new LeafToDoItemsViewModelSetting(this)
        );
    }

    public override ValueTask<Result> SetStateAsync(object setting)
    {
        return setting.CastObject<LeafToDoItemsViewModelSetting>()
            .IfSuccessAsync(
                s => this.InvokeUIBackgroundAsync(
                        () =>
                        {
                            ToDoSubItemsViewModel.List.GroupBy = s.GroupBy;
                            ToDoSubItemsViewModel.List.IsMulti = s.IsMulti;
                        }
                    )
                    .ConfigureAwait(false)
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