using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Ninject;
using ReactiveUI;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class SearchViewModel : NavigatableViewModelBase, IRefresh
{
    private string searchText = string.Empty;
    private readonly TaskWork refreshWork;

    public SearchViewModel() : base(true)
    {
        refreshWork = TaskWork.Create(RefreshCoreAsync);
        SearchCommand = CreateCommandFromTask(refreshWork.RunAsync);
    }

    public ICommand SearchCommand { get; }

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
    public required IMapper Mapper { get; init; }

    [Inject]
    public required MainSplitViewModel MainSplitViewModel { get; init; }

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
}