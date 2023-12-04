using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Avalonia.Threading;
using Ninject;
using ReactiveUI;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.Ui.Extensions;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class SearchViewModel : NavigatableViewModelBase, IRefresh
{
    private string searchText = string.Empty;

    public SearchViewModel() : base(true)
    {
        SearchCommand = CreateCommandFromTask(TaskWork.Create(RefreshAsync).RunAsync);
        SwitchPaneCommand = CreateCommand(SwitchPane);
    }

    public ICommand SearchCommand { get; }
    public ICommand SwitchPaneCommand { get; }

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

    private DispatcherOperation SwitchPane()
    {
        return this.InvokeUIAsync(() => MainSplitViewModel.IsPaneOpen = !MainSplitViewModel.IsPaneOpen);
    }

    public async Task RefreshAsync(CancellationToken cancellationToken)
    {
        var ids = await ToDoService.SearchToDoItemIdsAsync(SearchText, cancellationToken).ConfigureAwait(false);
        await ToDoSubItemsViewModel.UpdateItemsAsync(ids.ToArray(), this, cancellationToken).ConfigureAwait(false);
    }
}