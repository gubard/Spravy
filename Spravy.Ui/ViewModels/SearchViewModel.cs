using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Avalonia.Collections;
using ExtensionFramework.Core.Common.Extensions;
using ExtensionFramework.Core.DependencyInjection.Attributes;
using ExtensionFramework.ReactiveUI.Models;
using ReactiveUI;
using Spravy.Core.Interfaces;
using Spravy.Ui.Models;
using Spravy.Ui.Views;

namespace Spravy.Ui.ViewModels;

public class SearchViewModel : RoutableViewModelBase
{
    private string searchText = string.Empty;

    public SearchViewModel() : base("search")
    {
        SearchCommand = CreateCommandFromTask(RefreshToDoItemAsync);
        DeleteSubToDoItemCommand = CreateCommandFromTask<ToDoItemNotify>(DeleteSubToDoItemAsync);
        ChangeToDoItemCommand = CreateCommand<ToDoItemNotify>(ChangeToDoItem);
        CompleteSubToDoItemCommand = CreateCommandFromTask<ToDoItemNotify>(CompleteSubToDoItemAsync);
    }

    public AvaloniaList<ToDoItemNotify> SearchResult { get; } = new();
    public ICommand SearchCommand { get; }
    public ICommand DeleteSubToDoItemCommand { get; }
    public ICommand ChangeToDoItemCommand { get; }
    public ICommand CompleteSubToDoItemCommand { get; }
    public ICommand ChangeParentToDoItemCommand { get; }

    public string SearchText
    {
        get => searchText;
        set => this.RaiseAndSetIfChanged(ref searchText, value);
    }

    [Inject]
    public required IToDoService ToDoService { get; init; }

    [Inject]
    public required IMapper Mapper { get; init; }

    private void ChangeToDoItem(ToDoItemNotify item)
    {
        Navigator.NavigateTo<ToDoItemViewModel>(vm => vm.Id = item.Id);
    }

    private async Task DeleteSubToDoItemAsync(ToDoItemNotify item)
    {
        await ToDoService.DeleteToDoItemAsync(item.Id);
        await RefreshToDoItemAsync();
    }

    private async Task RefreshToDoItemAsync()
    {
        var items = await ToDoService.SearchAsync(SearchText);
        SearchResult.Clear();
        SearchResult.AddRange(items.Select(x => Mapper.Map<ToDoItemNotify>(x)));
    }

    private async Task CompleteSubToDoItemAsync(ToDoItemNotify item)
    {
        await DialogViewer.ShowDialogAsync<CompleteToDoItemView>(
            view =>
            {
                var viewModel = view.ViewModel.ThrowIfNull();
                viewModel.IsDialog = true;
                viewModel.Item = item;
            }
        );

        await RefreshToDoItemAsync();
    }
}