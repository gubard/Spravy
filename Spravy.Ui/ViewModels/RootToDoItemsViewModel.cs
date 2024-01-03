using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Ninject;
using ProtoBuf;
using Spravy.Domain.Extensions;
using Spravy.Domain.Helpers;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.Ui.Extensions;
using Spravy.Ui.Features.ToDo.Enums;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class RootToDoItemsViewModel : NavigatableViewModelBase, IToDoItemOrderChanger
{
    private readonly TaskWork refreshWork;

    public RootToDoItemsViewModel() : base(true)
    {
        refreshWork = TaskWork.Create(RefreshCoreAsync);
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }

    public ICommand InitializedCommand { get; }
    public override string ViewId => TypeCache<RootToDoItemsViewModel>.Type.Name;

    [Inject]
    public required PageHeaderViewModel PageHeaderViewModel { get; init; }

    [Inject]
    public required ToDoSubItemsViewModel ToDoSubItemsViewModel { get; init; }

    [Inject]
    public required IToDoService ToDoService { get; init; }

    [Inject]
    public required IObjectStorage ObjectStorage { get; init; }

    private async Task InitializedAsync(CancellationToken cancellationToken)
    {
        var setting = await ObjectStorage.GetObjectOrDefaultAsync<RootToDoItemsViewModelSetting>(ViewId)
            .ConfigureAwait(false);

        await SetStateAsync(setting).ConfigureAwait(false);
        await refreshWork.RunAsync().ConfigureAwait(false);
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

    public override void Stop()
    {
        refreshWork.Cancel();
    }

    public override Task SaveStateAsync()
    {
        return ObjectStorage.SaveObjectAsync(ViewId, new RootToDoItemsViewModelSetting(this));
    }

    public override async Task SetStateAsync(object setting)
    {
        var s = setting.ThrowIfIsNotCast<RootToDoItemsViewModelSetting>();

        await this.InvokeUIAsync(
            () =>
            {
                ToDoSubItemsViewModel.List.GroupBy = s.GroupBy;
                ToDoSubItemsViewModel.List.IsMulti = s.IsMulti;
            }
        );
    }

    [ProtoContract]
    class RootToDoItemsViewModelSetting
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