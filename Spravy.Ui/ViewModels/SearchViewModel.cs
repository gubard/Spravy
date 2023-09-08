using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Avalonia.Controls;
using Ninject;
using ReactiveUI;
using Spravy.Domain.Extensions;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.Ui.Extensions;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;
using Spravy.Ui.Views;

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
    public required ToDoSubItemsView ToDoSubItemsView { get; init; }

    [Inject]
    public required IToDoService ToDoService { get; init; }

    [Inject]
    public required IMapper Mapper { get; init; }

    [Inject]
    public required SplitView SplitView { get; init; }

    private void SwitchPane()
    {
        SplitView.IsPaneOpen = !SplitView.IsPaneOpen;
    }

    public async Task RefreshToDoItemAsync()
    {
        var items = await ToDoService.SearchToDoSubItemsAsync(SearchText);
        ToDoSubItemsView.ViewModel.ThrowIfNull().UpdateItems(Mapper.Map<IEnumerable<ToDoSubItemNotify>>(items), this);
    }
}