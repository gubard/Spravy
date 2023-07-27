using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Avalonia.Collections;
using ExtensionFramework.Core.Common.Extensions;
using ExtensionFramework.Core.DependencyInjection.Attributes;
using ExtensionFramework.ReactiveUI.Models;
using Spravy.Domain.Interfaces;
using Spravy.Ui.Extensions;
using Spravy.Ui.Models;
using Spravy.Ui.Views;

namespace Spravy.Ui.ViewModels;

public class CurrentDoToItemsViewModel : RoutableViewModelBase
{
    public CurrentDoToItemsViewModel() : base("current-do-to-items")
    {
        InitializedCommand = CreateCommandFromTaskWithDialogProgressIndicator(InitializedAsync);
        DeleteSubToDoItemCommand =
            CreateCommandFromTaskWithDialogProgressIndicator<ToDoSubItemNotify>(DeleteSubToDoItemAsync);
        CompleteSubToDoItemCommand =
            CreateCommandFromTaskWithDialogProgressIndicator<ToDoSubItemNotify>(CompleteSubToDoItemAsync);
        ChangeToDoItemCommand = CreateCommandFromTask<ToDoSubItemNotify>(ChangeToDoItemAsync);
        AddSubToDoItemToCurrentCommand =
            CreateCommandFromTaskWithDialogProgressIndicator<ToDoSubItemNotify>(AddCurrentToDoItemAsync);
        RemoveSubToDoItemFromCurrentCommand =
            CreateCommandFromTaskWithDialogProgressIndicator<ToDoSubItemNotify>(RemoveCurrentToDoItemAsync);
        ChangeToActiveDoItemCommand = CreateCommandFromTask<ActiveToDoItemNotify>(ChangeToActiveDoItemAsync);
    }

    public ICommand CompleteSubToDoItemCommand { get; }
    public ICommand DeleteSubToDoItemCommand { get; }
    public ICommand ChangeToDoItemCommand { get; }
    public ICommand ChangeToActiveDoItemCommand { get; }
    public ICommand AddSubToDoItemToCurrentCommand { get; }
    public ICommand RemoveSubToDoItemFromCurrentCommand { get; }
    public ICommand InitializedCommand { get; }
    public AvaloniaList<ToDoSubItemNotify> Items { get; } = new();

    [Inject]
    public required IToDoService ToDoService { get; init; }

    [Inject]
    public required IMapper Mapper { get; init; }

    private Task ChangeToActiveDoItemAsync(ActiveToDoItemNotify item)
    {
        return ToDoService.NavigateToToDoItemViewModel(item.Id, Navigator);
    }

    private Task InitializedAsync()
    {
        return RefreshToDoItemAsync();
    }

    private async Task RemoveCurrentToDoItemAsync(ToDoSubItemNotify item)
    {
        await ToDoService.RemoveCurrentToDoItemAsync(item.Id);
        await RefreshToDoItemAsync();
    }

    private async Task RefreshToDoItemAsync()
    {
        var items = await ToDoService.GetCurrentToDoItemsAsync();
        Items.Clear();
        Items.AddRange(Mapper.Map<IEnumerable<ToDoSubItemNotify>>(items));
    }

    private Task ChangeToDoItemAsync(ToDoSubItemNotify subItemValue)
    {
        return ToDoService.NavigateToToDoItemViewModel(subItemValue.Id, Navigator);
    }

    private async Task AddCurrentToDoItemAsync(ToDoSubItemNotify item)
    {
        await ToDoService.AddCurrentToDoItemAsync(item.Id);
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

    private async Task DeleteSubToDoItemAsync(ToDoSubItemNotify subItemValue)
    {
        await ToDoService.DeleteToDoItemAsync(subItemValue.Id);
        await RefreshToDoItemAsync();
    }
}