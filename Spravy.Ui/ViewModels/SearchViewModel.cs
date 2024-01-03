using System.Linq;
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
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class SearchViewModel : NavigatableViewModelBase, IToDoItemSearchProperties
{
    private string searchText = string.Empty;
    private readonly TaskWork refreshWork;

    public SearchViewModel() : base(true)
    {
        refreshWork = TaskWork.Create(RefreshCoreAsync);
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }

    public ICommand InitializedCommand { get; }
    public override string ViewId => TypeCache<SearchViewModel>.Type.Name;

    public string SearchText
    {
        get => searchText;
        set => this.RaiseAndSetIfChanged(ref searchText, value);
    }

    [Inject]
    public required ToDoSubItemsViewModel ToDoSubItemsViewModel { get; init; }

    [Inject]
    public required IToDoService ToDoService { get; init; }

    [Inject]
    public required IObjectStorage ObjectStorage { get; init; }

    private async Task InitializedAsync(CancellationToken cancellationToken)
    {
        var setting = await ObjectStorage.GetObjectOrDefaultAsync<SearchViewModelSetting>(ViewId).ConfigureAwait(false);
        await SetStateAsync(setting).ConfigureAwait(false);
    }

    public Task RefreshAsync(CancellationToken cancellationToken)
    {
        return refreshWork.RunAsync();
    }

    private async Task RefreshCoreAsync(CancellationToken cancellationToken)
    {
        var ids = await ToDoService.SearchToDoItemIdsAsync(SearchText, cancellationToken).ConfigureAwait(false);
        await ToDoSubItemsViewModel.UpdateItemsAsync(ids.ToArray(), this, cancellationToken).ConfigureAwait(false);
    }

    public override void Stop()
    {
        refreshWork.Cancel();
    }

    public override Task SaveStateAsync()
    {
        return ObjectStorage.SaveObjectAsync(ViewId, new SearchViewModelSetting(this));
    }

    public override async Task SetStateAsync(object setting)
    {
        var s = setting.ThrowIfIsNotCast<SearchViewModelSetting>();
        await this.InvokeUIAsync(() => SearchText = s.SearchText);
    }

    [ProtoContract]
    class SearchViewModelSetting
    {
        public SearchViewModelSetting(SearchViewModel viewModel)
        {
            SearchText = viewModel.SearchText;
        }

        public SearchViewModelSetting()
        {
        }

        [ProtoMember(1)]
        public string SearchText { get; set; } = string.Empty;
    }
}