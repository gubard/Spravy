using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Collections;
using ExtensionFramework.Core.Common.Extensions;
using ExtensionFramework.Core.DependencyInjection.Attributes;
using ExtensionFramework.ReactiveUI.Models;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.Ui.Extensions;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;
using Spravy.Ui.Views;

namespace Spravy.Ui.ViewModels;

public class ToDoSubItemsViewModel : ViewModelBase, IToDoItemOrderChanger
{
    private IRefreshToDoItem? refreshToDoItem;

    public ToDoSubItemsViewModel()
    {
        CompleteSubToDoItemCommand =
            CreateCommandFromTaskWithDialogProgressIndicator<ToDoSubItemNotify>(CompleteSubToDoItemAsync);
        DeleteSubToDoItemCommand = CreateCommandFromTask<ToDoSubItemNotify>(DeleteSubToDoItemAsync);
        ChangeToDoItemCommand = CreateCommandFromTask<ToDoSubItemNotify>(ChangeToDoItemAsync);
        AddSubToDoItemToCurrentCommand =
            CreateCommandFromTaskWithDialogProgressIndicator<ToDoSubItemNotify>(AddCurrentToDoItemAsync);
        RemoveSubToDoItemFromCurrentCommand =
            CreateCommandFromTaskWithDialogProgressIndicator<ToDoSubItemNotify>(RemoveCurrentToDoItemAsync);
        ChangeToActiveDoItemCommand = CreateCommandFromTask<ActiveToDoItemNotify>(ChangeToActiveDoItemAsync);
    }

    public AvaloniaList<ToDoSubItemNotify> Missed { get; } = new();
    public AvaloniaList<ToDoSubItemNotify> ReadyForCompleted { get; } = new();
    public AvaloniaList<ToDoSubItemNotify> Completed { get; } = new();
    public ICommand CompleteSubToDoItemCommand { get; }
    public ICommand DeleteSubToDoItemCommand { get; }
    public ICommand ChangeToDoItemCommand { get; }
    public ICommand AddSubToDoItemToCurrentCommand { get; }
    public ICommand RemoveSubToDoItemFromCurrentCommand { get; }
    public ICommand ChangeToActiveDoItemCommand { get; }

    [Inject]
    public required IToDoService ToDoService { get; set; }

    public Task RefreshToDoItemAsync()
    {
        return refreshToDoItem.ThrowIfNull().RefreshToDoItemAsync();
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

        await refreshToDoItem.ThrowIfNull().RefreshToDoItemAsync();
    }

    private async Task RemoveCurrentToDoItemAsync(ToDoSubItemNotify item)
    {
        await ToDoService.RemoveCurrentToDoItemAsync(item.Id);
        await refreshToDoItem.ThrowIfNull().RefreshToDoItemAsync();
    }

    private Task DeleteSubToDoItemAsync(ToDoSubItemNotify subItem)
    {
        return DialogViewer.ShowConfirmDialogAsync<DeleteToDoItemView>(
            async _ => DialogViewer.CloseDialog(),
            async view =>
            {
                await ToDoService.DeleteToDoItemAsync(view.ViewModel.ThrowIfNull().Item.ThrowIfNull().Id);
                DialogViewer.CloseDialog();
                await refreshToDoItem.ThrowIfNull().RefreshToDoItemAsync();
            },
            view => view.ViewModel.ThrowIfNull().Item = subItem
        );
    }

    private Task ChangeToDoItemAsync(ToDoSubItemNotify subItemValue)
    {
        return ToDoService.NavigateToToDoItemViewModel(subItemValue.Id, Navigator);
    }

    private async Task AddCurrentToDoItemAsync(ToDoSubItemNotify item)
    {
        await ToDoService.AddCurrentToDoItemAsync(item.Id);
        await refreshToDoItem.ThrowIfNull().RefreshToDoItemAsync();
    }

    private Task ChangeToActiveDoItemAsync(ActiveToDoItemNotify item)
    {
        return ToDoService.NavigateToToDoItemViewModel(item.Id, Navigator);
    }

    public void UpdateItems(IEnumerable<ToDoSubItemNotify> items, IRefreshToDoItem refresh)
    {
        var itemsArray = items.ToArray();
        refreshToDoItem = refresh;
        Missed.Clear();
        Missed.AddRange(itemsArray.Where(x => x.Status == ToDoItemStatus.Miss));
        ReadyForCompleted.Clear();
        ReadyForCompleted.AddRange(itemsArray.Where(x => x.Status == ToDoItemStatus.ReadyForComplete));
        Completed.Clear();
        Completed.AddRange(itemsArray.Where(x => x.Status == ToDoItemStatus.Completed));
    }
}