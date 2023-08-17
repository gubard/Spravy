using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using ExtensionFramework.Core.Common.Extensions;
using ExtensionFramework.Core.DependencyInjection.Attributes;
using ExtensionFramework.ReactiveUI.Models;
using ReactiveUI;
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
        CompleteSubToDoItemCommand =
            CreateCommandFromTaskWithDialogProgressIndicator<ToDoSubItemNotify>(CompleteSubToDoItemAsync);
        ChangeToActiveDoItemCommand = CreateCommandFromTask<ActiveToDoItemNotify>(ChangeToActiveDoItem);
        DeleteSubToDoItemCommand =
            CreateCommandFromTaskWithDialogProgressIndicator<ToDoSubItemNotify>(DeleteSubToDoItemAsync);
        ChangeToDoItemCommand = CreateCommandFromTask<ToDoSubItemNotify>(ChangeToDoItem);
    }

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
    public required ToDoSubItemsView ToDoSubItemsView { get; init; }

    [Inject]
    public required IToDoService ToDoService { get; init; }

    [Inject]
    public required IMapper Mapper { get; init; }

    public async Task RefreshToDoItemAsync()
    {
        var items = await ToDoService.SearchToDoSubItemsAsync(SearchText);
        ToDoSubItemsView.ViewModel.ThrowIfNull().UpdateItems(Mapper.Map<IEnumerable<ToDoSubItemNotify>>(items), this);
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