using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Ninject;
using ReactiveUI;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class SearchViewModel : RoutableViewModelBase, IRefreshToDoItem
{
    private string searchText = string.Empty;

    public SearchViewModel() : base("search")
    {
        SearchCommand = CreateCommandFromTaskWithDialogProgressIndicator(RefreshToDoItemAsync);
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

    private void SwitchPane()
    {
        MainSplitViewModel.IsPaneOpen = !MainSplitViewModel.IsPaneOpen;
    }

    public async Task RefreshToDoItemAsync()
    {
        var items = await ToDoService.SearchToDoSubItemsAsync(SearchText).ConfigureAwait(false);
        await ToDoSubItemsViewModel.UpdateItemsAsync(Mapper.Map<IEnumerable<ToDoSubItemNotify>>(items), this);
    }
}