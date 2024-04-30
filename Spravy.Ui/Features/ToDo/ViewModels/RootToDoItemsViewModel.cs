using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
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
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Features.ToDo.ViewModels;

public class RootToDoItemsViewModel : NavigatableViewModelBase, IToDoItemOrderChanger
{
    private readonly PageHeaderViewModel pageHeaderViewModel;
    private readonly TaskWork refreshWork;
    private readonly ToDoSubItemsViewModel toDoSubItemsViewModel;

    public RootToDoItemsViewModel() : base(true)
    {
        refreshWork = TaskWork.Create(RefreshCoreAsync);
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }

    public ICommand InitializedCommand { get; }

    public override string ViewId
    {
        get => TypeCache<RootToDoItemsViewModel>.Type.Name;
    }

    [Inject]
    public required PageHeaderViewModel PageHeaderViewModel
    {
        get => pageHeaderViewModel;
        [MemberNotNull(nameof(pageHeaderViewModel))]
        init
        {
            pageHeaderViewModel = value;
            pageHeaderViewModel.Header = "Spravy";
            pageHeaderViewModel.LeftCommand = CommandStorage.NavigateToCurrentToDoItemItem;
            Disposables.Add(pageHeaderViewModel);
        }
    }

    [Inject]
    public required ToDoSubItemsViewModel ToDoSubItemsViewModel
    {
        get => toDoSubItemsViewModel;
        [MemberNotNull(nameof(toDoSubItemsViewModel))]
        init
        {
            toDoSubItemsViewModel = value;

            toDoSubItemsViewModel.List
               .WhenAnyValue(x => x.IsMulti)
               .Skip(1)
               .Subscribe(x =>
                {
                    if (x)
                    {
                        PageHeaderViewModel.SetMultiCommands(ToDoSubItemsViewModel);
                    }
                });
        }
    }

    [Inject]
    public required IObjectStorage ObjectStorage { get; init; }

    [Inject]
    public required IToDoService ToDoService { get; init; }

    public ConfiguredValueTaskAwaitable<Result> RefreshAsync(CancellationToken cancellationToken)
    {
        return RefreshCore(cancellationToken).ConfigureAwait(false);
    }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        return ObjectStorage.GetObjectOrDefaultAsync<RootToDoItemsViewModelSetting>(ViewId, cancellationToken)
           .IfSuccessAllAsync(cancellationToken, obj => SetStateAsync(obj, cancellationToken),
                _ => refreshWork.RunAsync().ToValueTaskResultOnly().ConfigureAwait(false));
    }

    public async ValueTask<Result> RefreshCore(CancellationToken cancellationToken)
    {
        await refreshWork.RunAsync();

        return Result.Success;
    }

    private ConfiguredValueTaskAwaitable<Result> RefreshCoreAsync(CancellationToken cancellationToken)
    {
        return ToDoService.GetRootToDoItemIdsAsync(cancellationToken)
           .IfSuccessAsync(ids => ToDoSubItemsViewModel.UpdateItemsAsync(ids.ToArray(), this, false, cancellationToken),
                cancellationToken);
    }

    public override Result Stop()
    {
        refreshWork.Cancel();

        return Result.Success;
    }

    public override ConfiguredValueTaskAwaitable<Result> SaveStateAsync(CancellationToken cancellationToken)
    {
        return ObjectStorage.SaveObjectAsync(ViewId, new RootToDoItemsViewModelSetting(this));
    }

    public override ConfiguredValueTaskAwaitable<Result> SetStateAsync(
        object setting,
        CancellationToken cancellationToken
    )
    {
        return setting.CastObject<RootToDoItemsViewModelSetting>()
           .IfSuccessAsync(s => this.InvokeUIBackgroundAsync(() =>
            {
                ToDoSubItemsViewModel.List.GroupBy = s.GroupBy;
                ToDoSubItemsViewModel.List.IsMulti = s.IsMulti;
            }), cancellationToken);
    }

    [ProtoContract]
    private class RootToDoItemsViewModelSetting
    {
        public RootToDoItemsViewModelSetting(RootToDoItemsViewModel viewModel)
        {
            GroupBy = viewModel.ToDoSubItemsViewModel.List.GroupBy;
            IsMulti = viewModel.ToDoSubItemsViewModel.List.IsMulti;
        }

        public RootToDoItemsViewModelSetting()
        {
        }

        [ProtoMember(1)]
        public GroupBy GroupBy { get; set; } = GroupBy.ByStatus;

        [ProtoMember(2)]
        public bool IsMulti { get; set; }
    }
}