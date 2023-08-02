using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Avalonia.Collections;
using ExtensionFramework.Core.Common.Extensions;
using ExtensionFramework.Core.DependencyInjection.Attributes;
using ExtensionFramework.ReactiveUI.Models;
using ReactiveUI;
using Spravy.Domain.Interfaces;
using Spravy.Ui.Extensions;
using Spravy.Ui.Models;
using Spravy.Ui.Views;

namespace Spravy.Ui.ViewModels;

public class SearchViewModel : RoutableViewModelBase
{
    private string searchText = string.Empty;

    public SearchViewModel() : base("search")
    {
        SearchCommand = CreateCommandFromTaskWithDialogProgressIndicator(RefreshToDoItemAsync);
        CompleteSubToDoItemCommand = CreateCommandFromTaskWithDialogProgressIndicator<ToDoSubItemNotify>(CompleteSubToDoItemAsync);
        ChangeToActiveDoItemCommand = CreateCommandFromTask<ActiveToDoItemNotify>(ChangeToActiveDoItem);
        DeleteSubToDoItemCommand = CreateCommandFromTaskWithDialogProgressIndicator<ToDoSubItemNotify>(DeleteSubToDoItemAsync);
        ChangeToDoItemCommand = CreateCommandFromTask<ToDoSubItemNotify>(ChangeToDoItem);
    }

    public AvaloniaList<ToDoSubItemNotify> SearchResult { get; } = new();
    public ICommand SearchCommand { get; }
    public ICommand DeleteSubToDoItemCommand { get; }
    public ICommand ChangeToDoItemCommand { get; }
    public ICommand ChangeToActiveDoItemCommand { get; }
    public ICommand CompleteSubToDoItemCommand { get; }

    public string SearchText
    {
        get => searchText;
        set => this.RaiseAndSetIfChanged(ref searchText, value);
    }

    [Inject]
    public required IToDoService ToDoService { get; init; }

    [Inject]
    public required IMapper Mapper { get; init; }
    
    private async Task RefreshToDoItemAsync()
    {
        var items = await ToDoService.SearchToDoSubItemsAsync(SearchText);
        SearchResult.Clear();
        SearchResult.AddRange(items.Select(x => Mapper.Map<ToDoSubItemNotify>(x)));
    }
    
    private Task ChangeToActiveDoItem(ActiveToDoItemNotify item)
    {
        return ToDoService.NavigateToToDoItemViewModel(item.Id, Navigator);
    }

    private Task ChangeToDoItem(ToDoSubItemNotify subItemValue)
    {
        return ToDoService.NavigateToToDoItemViewModel(subItemValue.Id, Navigator);
    }

    private async Task DeleteSubToDoItemAsync(ToDoSubItemNotify subItemValue)
    {
        await ToDoService.DeleteToDoItemAsync(subItemValue.Id);
        await RefreshToDoItemAsync();
    }

    private async Task CompleteSubToDoItemAsync(ToDoSubItemNotify subItemValue)
    {
        await DialogViewer.ShowDialogAsync<CompleteToDoItemView>(
            view =>
            {
                var viewModel = view.ViewModel.ThrowIfNull();
                viewModel.IsDialog = true;
                viewModel.Item = subItemValue;
            }
        );

        await RefreshToDoItemAsync();
    }
}