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
using Spravy.Ui.Models;
using Spravy.Ui.Views;

namespace Spravy.Ui.ViewModels;

public class SearchViewModel : RoutableViewModelBase
{
    private string searchText = string.Empty;

    public SearchViewModel() : base("search")
    {
        SearchCommand = CreateCommandFromTask(RefreshToDoItemAsync);
        DeleteSubToDoItemCommand = CreateCommandFromTask<ToDoSubItemNotify>(DeleteSubToDoItemAsync);
        ChangeToDoItemCommand = CreateCommand<ToDoSubItemNotify>(ChangeToDoItem);
        CompleteSubToDoItemCommand = CreateCommandFromTask<ToDoSubItemValueNotify>(CompleteSubToDoItemAsync);
        ChangeToActiveDoItemCommand = CreateCommand<ActiveToDoItemNotify>(ChangeToActiveDoItem);
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
    
    private void ChangeToActiveDoItem(ActiveToDoItemNotify item)
    {
        Navigator.NavigateTo<ToDoItemValueViewModel>(vm => vm.Id = item.Id);
    }

    private void ChangeToDoItem(ToDoSubItemNotify subItemValue)
    {
        Navigator.NavigateTo<ToDoItemValueViewModel>(vm => vm.Id = subItemValue.Id);
    }

    private async Task DeleteSubToDoItemAsync(ToDoSubItemNotify subItemValue)
    {
        await ToDoService.DeleteToDoItemAsync(subItemValue.Id);
        await RefreshToDoItemAsync();
    }

    private async Task RefreshToDoItemAsync()
    {
        var items = await ToDoService.SearchAsync(SearchText);
        SearchResult.Clear();
        SearchResult.AddRange(items.Select(x => Mapper.Map<ToDoSubItemNotify>(x)));
    }

    private async Task CompleteSubToDoItemAsync(ToDoSubItemValueNotify subItemValue)
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