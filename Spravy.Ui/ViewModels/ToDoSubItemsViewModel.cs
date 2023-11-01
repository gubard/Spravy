using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Avalonia.Collections;
using Ninject;
using Spravy.Domain.Extensions;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.ToDo.Domain.Models;
using Spravy.Ui.Enums;
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
        CompleteSubToDoItemCommand = CreateCommandFromTask<ToDoSubItemNotify>(CompleteSubToDoItemAsync);
        DeleteSubToDoItemCommand = CreateCommandFromTask<ToDoSubItemNotify>(DeleteSubToDoItemAsync);
        ChangeToDoItemCommand = CreateCommandFromTask<ToDoSubItemNotify>(ChangeToDoItemAsync);

        AddSubToDoItemToFavoriteCommand =
            CreateCommandFromTaskWithDialogProgressIndicator<ToDoSubItemNotify>(AddFavoriteToDoItemAsync);

        RemoveSubToDoItemFromFavoriteCommand =
            CreateCommandFromTaskWithDialogProgressIndicator<ToDoSubItemNotify>(RemoveFavoriteToDoItemAsync);

        ChangeToActiveDoItemCommand = CreateCommandFromTask<ActiveToDoItemNotify>(ChangeToActiveDoItemAsync);
        InitializedCommand = CreateInitializedCommand(InitializedAsync);
        CompleteSelectedToDoItemsCommand = CreateCommandFromTask(CompleteSelectedToDoItemsAsync);
        ChangeOrderIndexCommand = CreateCommandFromTask<ToDoSubItemNotify>(ChangeOrderIndexAsync);
    }

    public AvaloniaList<ToDoSubItemNotify> Missed { get; } = new();
    public AvaloniaList<ToDoSubItemNotify> Planned { get; } = new();
    public AvaloniaList<ToDoSubItemNotify> ReadyForCompleted { get; } = new();
    public AvaloniaList<ToDoSubItemNotify> Completed { get; } = new();
    public AvaloniaList<ToDoSubItemNotify> FavoriteToDoItems { get; } = new();
    public AvaloniaList<ToDoSubItemNotify> SelectedMissed { get; } = new();
    public AvaloniaList<ToDoSubItemNotify> SelectedPlanned { get; } = new();
    public AvaloniaList<ToDoSubItemNotify> SelectedReadyForCompleted { get; } = new();
    public AvaloniaList<ToDoSubItemNotify> SelectedCompleted { get; } = new();
    public AvaloniaList<ToDoSubItemNotify> SelectedFavoriteToDoItems { get; } = new();
    public ICommand CompleteSubToDoItemCommand { get; }
    public ICommand DeleteSubToDoItemCommand { get; }
    public ICommand ChangeToDoItemCommand { get; }
    public ICommand AddSubToDoItemToFavoriteCommand { get; }
    public ICommand RemoveSubToDoItemFromFavoriteCommand { get; }
    public ICommand ChangeToActiveDoItemCommand { get; }
    public ICommand InitializedCommand { get; }
    public ICommand CompleteSelectedToDoItemsCommand { get; }
    public ICommand ChangeOrderIndexCommand { get; }

    [Inject]
    public required IToDoService ToDoService { get; set; }

    [Inject]
    public required IMapper Mapper { get; set; }

    private Task ChangeOrderIndexAsync(ToDoSubItemNotify item)
    {
        return DialogViewer.ShowConfirmContentDialogAsync<ChangeToDoItemOrderIndexView>(
            async view =>
            {
                var viewModel = view.ViewModel.ThrowIfNull();
                var targetId = viewModel.SelectedItem.ThrowIfNull().Id;
                var options = new UpdateOrderIndexToDoItemOptions(viewModel.Id, targetId, viewModel.IsAfter);
                await ToDoService.UpdateToDoItemOrderIndexAsync(options);
                await RefreshToDoItemAsync();
                await DialogViewer.CloseContentDialogAsync();
            },
            _ => DialogViewer.CloseContentDialogAsync(),
            view =>
            {
                var viewModel = view.ViewModel.ThrowIfNull();
                viewModel.Id = item.Id;
            }
        );
    }

    private Task CompleteSelectedToDoItemsAsync()
    {
        return DialogViewer.ShowInfoInputDialogAsync<CompleteToDoItemView>(
            _ => DialogViewer.CloseInputDialogAsync(),
            view =>
            {
                var viewModel = view.ViewModel.ThrowIfNull();
                viewModel.SetAllStatus();

                viewModel.Complete = async status =>
                {
                    await CompleteAsync(SelectedCompleted, status);
                    await CompleteAsync(SelectedMissed, status);
                    await CompleteAsync(SelectedFavoriteToDoItems, status);
                    await CompleteAsync(SelectedReadyForCompleted, status);
                    await CompleteAsync(SelectedPlanned, status);
                    await RefreshToDoItemAsync();
                    await DialogViewer.CloseInputDialogAsync();
                };
            }
        );
    }

    public Task RefreshToDoItemAsync()
    {
        return Task.WhenAll(InitializedAsync(), refreshToDoItem.ThrowIfNull().RefreshToDoItemAsync());
    }

    private Task CompleteSubToDoItemAsync(ToDoSubItemNotify subItemValue)
    {
        return DialogViewer.ShowInfoInputDialogAsync<CompleteToDoItemView>(
            _ => DialogViewer.CloseInputDialogAsync(),
            view =>
            {
                var viewModel = view.ViewModel.ThrowIfNull();

                switch(subItemValue)
                {
                    case ToDoSubItemPeriodicityNotify:
                        viewModel.SetCompleteStatus();

                        break;
                    case ToDoSubItemPeriodicityOffsetNotify:
                        viewModel.SetCompleteStatus();

                        break;
                    case ToDoSubItemPlannedNotify toDoSubItemPlannedNotify:
                        if(toDoSubItemPlannedNotify.IsCompleted)
                        {
                            viewModel.SetIncompleteStatus();
                        }
                        else
                        {
                            viewModel.SetCompleteStatus();
                        }

                        break;
                    case ToDoSubItemValueNotify toDoSubItemValueNotify:
                        if(toDoSubItemValueNotify.IsCompleted)
                        {
                            viewModel.SetIncompleteStatus();
                        }
                        else
                        {
                            viewModel.SetCompleteStatus();
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(subItemValue));
                }

                viewModel.Complete = async status =>
                {
                    switch(status)
                    {
                        case CompleteStatus.Complete:
                            await ToDoService.UpdateToDoItemCompleteStatusAsync(subItemValue.Id, true);

                            break;
                        case CompleteStatus.Incomplete:
                            await ToDoService.UpdateToDoItemCompleteStatusAsync(subItemValue.Id, false);

                            break;
                        case CompleteStatus.Skip:
                            await ToDoService.SkipToDoItemAsync(subItemValue.Id);

                            break;
                        case CompleteStatus.Fail:
                            await ToDoService.FailToDoItemAsync(subItemValue.Id);

                            break;
                        default: throw new ArgumentOutOfRangeException(nameof(status), status, null);
                    }

                    await RefreshToDoItemAsync();
                    await DialogViewer.CloseInputDialogAsync();
                };
            }
        );
    }

    private async Task CompleteAsync(IEnumerable<ToDoSubItemNotify> items, CompleteStatus status)
    {
        switch(status)
        {
            case CompleteStatus.Complete:
                foreach(var item in items)
                {
                    switch(item)
                    {
                        case ToDoSubItemPeriodicityNotify:
                            await ToDoService.UpdateToDoItemCompleteStatusAsync(item.Id, true);

                            break;
                        case ToDoSubItemPeriodicityOffsetNotify:
                            await ToDoService.UpdateToDoItemCompleteStatusAsync(item.Id, true);

                            break;
                        case ToDoSubItemPlannedNotify toDoSubItemPlannedNotify:
                            if(!toDoSubItemPlannedNotify.IsCompleted)
                            {
                                await ToDoService.UpdateToDoItemCompleteStatusAsync(item.Id, true);
                            }
                            else
                            {
                                throw new ArgumentException(nameof(item));
                            }

                            break;
                        case ToDoSubItemValueNotify toDoSubItemValueNotify:
                            if(!toDoSubItemValueNotify.IsCompleted)
                            {
                                await ToDoService.UpdateToDoItemCompleteStatusAsync(item.Id, true);
                            }
                            else
                            {
                                throw new ArgumentException(nameof(item));
                            }

                            break;
                        default: throw new ArgumentOutOfRangeException(nameof(item));
                    }
                }

                break;
            case CompleteStatus.Skip:
                foreach(var item in items)
                {
                    switch(item)
                    {
                        case ToDoSubItemPeriodicityNotify:
                            await ToDoService.SkipToDoItemAsync(item.Id);

                            break;
                        case ToDoSubItemPeriodicityOffsetNotify:
                            await ToDoService.SkipToDoItemAsync(item.Id);

                            break;
                        case ToDoSubItemPlannedNotify:
                            await ToDoService.SkipToDoItemAsync(item.Id);

                            break;
                        case ToDoSubItemValueNotify:
                            await ToDoService.SkipToDoItemAsync(item.Id);

                            break;
                        default: throw new ArgumentOutOfRangeException(nameof(item));
                    }
                }

                break;
            case CompleteStatus.Fail:
                foreach(var item in items)
                {
                    switch(item)
                    {
                        case ToDoSubItemPeriodicityNotify:
                            await ToDoService.FailToDoItemAsync(item.Id);

                            break;
                        case ToDoSubItemPeriodicityOffsetNotify:
                            await ToDoService.FailToDoItemAsync(item.Id);

                            break;
                        case ToDoSubItemPlannedNotify:
                            await ToDoService.FailToDoItemAsync(item.Id);

                            break;
                        case ToDoSubItemValueNotify:
                            await ToDoService.FailToDoItemAsync(item.Id);

                            break;
                        default: throw new ArgumentOutOfRangeException(nameof(item));
                    }
                }

                break;
            case CompleteStatus.Incomplete:
                foreach(var item in items)
                {
                    switch(item)
                    {
                        case ToDoSubItemPlannedNotify toDoSubItemPlannedNotify:
                            if(!toDoSubItemPlannedNotify.IsCompleted)
                            {
                                throw new ArgumentException(nameof(item));
                            }

                            await ToDoService.UpdateToDoItemCompleteStatusAsync(item.Id, true);

                            break;
                        case ToDoSubItemValueNotify toDoSubItemValueNotify:
                            if(!toDoSubItemValueNotify.IsCompleted)
                            {
                                throw new ArgumentException(nameof(item));
                            }

                            await ToDoService.UpdateToDoItemCompleteStatusAsync(item.Id, true);

                            break;
                        default: throw new ArgumentOutOfRangeException(nameof(item));
                    }
                }

                break;
            default: throw new ArgumentOutOfRangeException(nameof(status), status, null);
        }
    }

    private async Task RemoveFavoriteToDoItemAsync(ToDoSubItemNotify item)
    {
        await ToDoService.RemoveFavoriteToDoItemAsync(item.Id);
        await RefreshToDoItemAsync();
    }

    private async Task InitializedAsync()
    {
        var favoriteToDoItems = await ToDoService.GetFavoriteToDoItemsAsync();
        FavoriteToDoItems.Clear();
        FavoriteToDoItems.AddRange(Mapper.Map<IEnumerable<ToDoSubItemNotify>>(favoriteToDoItems));
    }

    private Task DeleteSubToDoItemAsync(ToDoSubItemNotify subItem)
    {
        return DialogViewer.ShowConfirmContentDialogAsync<DeleteToDoItemView>(
            async view =>
            {
                await ToDoService.DeleteToDoItemAsync(view.ViewModel.ThrowIfNull().Item.ThrowIfNull().Id);
                await DialogViewer.CloseContentDialogAsync();
                await RefreshToDoItemAsync();
            },
            _ => DialogViewer.CloseContentDialogAsync(),
            view => view.ViewModel.ThrowIfNull().Item = subItem
        );
    }

    private Task ChangeToDoItemAsync(ToDoSubItemNotify subItemValue)
    {
        return ToDoService.NavigateToToDoItemViewModel(subItemValue.Id, Navigator);
    }

    private async Task AddFavoriteToDoItemAsync(ToDoSubItemNotify item)
    {
        await ToDoService.AddFavoriteToDoItemAsync(item.Id);
        await RefreshToDoItemAsync();
    }

    private Task ChangeToActiveDoItemAsync(ActiveToDoItemNotify item)
    {
        return ToDoService.NavigateToToDoItemViewModel(item.Id, Navigator);
    }

    public Task UpdateItemsAsync(IEnumerable<ToDoSubItemNotify> items, IRefreshToDoItem refresh)
    {
        var itemsArray = items.ToArray();
        refreshToDoItem = refresh;
        Missed.Clear();
        Missed.AddRange(itemsArray.Where(x => x.Status == ToDoItemStatus.Miss));
        ReadyForCompleted.Clear();
        ReadyForCompleted.AddRange(itemsArray.Where(x => x.Status == ToDoItemStatus.ReadyForComplete));
        Completed.Clear();
        Completed.AddRange(itemsArray.Where(x => x.Status == ToDoItemStatus.Completed));
        Planned.Clear();
        Planned.AddRange(itemsArray.Where(x => x.Status == ToDoItemStatus.Planned));

        return InitializedAsync();
    }
}