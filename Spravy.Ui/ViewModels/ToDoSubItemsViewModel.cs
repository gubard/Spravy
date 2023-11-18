using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Avalonia.Collections;
using Avalonia.Threading;
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
        OpenLinkCommand = CreateCommandFromTask<ToDoSubItemNotify>(OpenLinkAsync);
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
    public ICommand OpenLinkCommand { get; }

    [Inject]
    public required IOpenerLink OpenerLink { get; set; }

    [Inject]
    public required IToDoService ToDoService { get; set; }

    [Inject]
    public required IMapper Mapper { get; set; }

    private Task OpenLinkAsync(ToDoSubItemNotify item)
    {
        var link = item.Link.ThrowIfNull();

        return OpenerLink.OpenLinkAsync(link);
    }

    private Task ChangeOrderIndexAsync(ToDoSubItemNotify item)
    {
        return DialogViewer.ShowConfirmContentDialogAsync<ChangeToDoItemOrderIndexView>(
            async view =>
            {
                var viewModel = view.ViewModel.ThrowIfNull();
                var targetId = viewModel.SelectedItem.ThrowIfNull().Id;
                var options = new UpdateOrderIndexToDoItemOptions(viewModel.Id, targetId, viewModel.IsAfter);
                await ToDoService.UpdateToDoItemOrderIndexAsync(options).ConfigureAwait(false);
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
                    await CompleteAsync(SelectedCompleted, status).ConfigureAwait(false);
                    await CompleteAsync(SelectedMissed, status).ConfigureAwait(false);
                    await CompleteAsync(SelectedFavoriteToDoItems, status).ConfigureAwait(false);
                    await CompleteAsync(SelectedReadyForCompleted, status).ConfigureAwait(false);
                    await CompleteAsync(SelectedPlanned, status).ConfigureAwait(false);
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

                switch (subItemValue)
                {
                    case ToDoSubItemPeriodicityNotify:
                        viewModel.SetCompleteStatus();

                        break;
                    case ToDoSubItemPeriodicityOffsetNotify:
                        viewModel.SetCompleteStatus();

                        break;
                    case ToDoSubItemPlannedNotify planned:
                        if (planned.IsCompleted)
                        {
                            viewModel.SetIncompleteStatus();
                        }
                        else
                        {
                            viewModel.SetCompleteStatus();
                        }

                        break;
                    case ToDoSubItemValueNotify value:
                        if (value.IsCompleted)
                        {
                            viewModel.SetIncompleteStatus();
                        }
                        else
                        {
                            viewModel.SetCompleteStatus();
                        }

                        break;
                    case ToDoSubItemCircleNotify circle:
                        if (circle.IsCompleted)
                        {
                            viewModel.SetIncompleteStatus();
                        }
                        else
                        {
                            viewModel.SetCompleteStatus();
                        }

                        break;
                    case ToDoSubItemStepNotify circle:
                        if (circle.IsCompleted)
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
                    switch (status)
                    {
                        case CompleteStatus.Complete:
                            await ToDoService.UpdateToDoItemCompleteStatusAsync(subItemValue.Id, true)
                                .ConfigureAwait(false);

                            break;
                        case CompleteStatus.Incomplete:
                            await ToDoService.UpdateToDoItemCompleteStatusAsync(subItemValue.Id, false)
                                .ConfigureAwait(false);

                            break;
                        case CompleteStatus.Skip:
                            await ToDoService.SkipToDoItemAsync(subItemValue.Id).ConfigureAwait(false);

                            break;
                        case CompleteStatus.Fail:
                            await ToDoService.FailToDoItemAsync(subItemValue.Id).ConfigureAwait(false);

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
        switch (status)
        {
            case CompleteStatus.Complete:
                foreach (var item in items)
                {
                    switch (item)
                    {
                        case ToDoSubItemPeriodicityNotify:
                            await ToDoService.UpdateToDoItemCompleteStatusAsync(item.Id, true).ConfigureAwait(false);

                            break;
                        case ToDoSubItemPeriodicityOffsetNotify:
                            await ToDoService.UpdateToDoItemCompleteStatusAsync(item.Id, true).ConfigureAwait(false);

                            break;
                        case ToDoSubItemPlannedNotify toDoSubItemPlannedNotify:
                            if (!toDoSubItemPlannedNotify.IsCompleted)
                            {
                                await ToDoService.UpdateToDoItemCompleteStatusAsync(item.Id, true)
                                    .ConfigureAwait(false);
                            }
                            else
                            {
                                throw new ArgumentException(nameof(item));
                            }

                            break;
                        case ToDoSubItemValueNotify toDoSubItemValueNotify:
                            if (!toDoSubItemValueNotify.IsCompleted)
                            {
                                await ToDoService.UpdateToDoItemCompleteStatusAsync(item.Id, true)
                                    .ConfigureAwait(false);
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
                foreach (var item in items)
                {
                    switch (item)
                    {
                        case ToDoSubItemPeriodicityNotify:
                            await ToDoService.SkipToDoItemAsync(item.Id).ConfigureAwait(false);

                            break;
                        case ToDoSubItemPeriodicityOffsetNotify:
                            await ToDoService.SkipToDoItemAsync(item.Id).ConfigureAwait(false);

                            break;
                        case ToDoSubItemPlannedNotify:
                            await ToDoService.SkipToDoItemAsync(item.Id).ConfigureAwait(false);

                            break;
                        case ToDoSubItemValueNotify:
                            await ToDoService.SkipToDoItemAsync(item.Id).ConfigureAwait(false);

                            break;
                        default: throw new ArgumentOutOfRangeException(nameof(item));
                    }
                }

                break;
            case CompleteStatus.Fail:
                foreach (var item in items)
                {
                    switch (item)
                    {
                        case ToDoSubItemPeriodicityNotify:
                            await ToDoService.FailToDoItemAsync(item.Id).ConfigureAwait(false);

                            break;
                        case ToDoSubItemPeriodicityOffsetNotify:
                            await ToDoService.FailToDoItemAsync(item.Id).ConfigureAwait(false);

                            break;
                        case ToDoSubItemPlannedNotify:
                            await ToDoService.FailToDoItemAsync(item.Id).ConfigureAwait(false);

                            break;
                        case ToDoSubItemValueNotify:
                            await ToDoService.FailToDoItemAsync(item.Id).ConfigureAwait(false);

                            break;
                        default: throw new ArgumentOutOfRangeException(nameof(item));
                    }
                }

                break;
            case CompleteStatus.Incomplete:
                foreach (var item in items)
                {
                    switch (item)
                    {
                        case ToDoSubItemPlannedNotify toDoSubItemPlannedNotify:
                            if (!toDoSubItemPlannedNotify.IsCompleted)
                            {
                                throw new ArgumentException(nameof(item));
                            }

                            await ToDoService.UpdateToDoItemCompleteStatusAsync(item.Id, true);

                            break;
                        case ToDoSubItemValueNotify toDoSubItemValueNotify:
                            if (!toDoSubItemValueNotify.IsCompleted)
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
        await ToDoService.RemoveFavoriteToDoItemAsync(item.Id).ConfigureAwait(false);
        await RefreshToDoItemAsync();
    }

    private async Task InitializedAsync()
    {
        var favoriteToDoItems = await ToDoService.GetFavoriteToDoItemsAsync().ConfigureAwait(false);

        await Dispatcher.UIThread.InvokeAsync(
            () =>
            {
                FavoriteToDoItems.Clear();
                FavoriteToDoItems.AddRange(Mapper.Map<IEnumerable<ToDoSubItemNotify>>(favoriteToDoItems));
            }
        );
    }

    private Task DeleteSubToDoItemAsync(ToDoSubItemNotify subItem)
    {
        return DialogViewer.ShowConfirmContentDialogAsync<DeleteToDoItemViewModel>(
            async view =>
            {
                await ToDoService.DeleteToDoItemAsync(view.Item.ThrowIfNull().Id).ConfigureAwait(false);
                await DialogViewer.CloseContentDialogAsync();
                await RefreshToDoItemAsync();
            },
            _ => DialogViewer.CloseContentDialogAsync(),
            view => view.Item = subItem
        );
    }

    private Task ChangeToDoItemAsync(ToDoSubItemNotify subItemValue)
    {
        return ToDoService.NavigateToToDoItemViewModel(subItemValue.Id, Navigator);
    }

    private async Task AddFavoriteToDoItemAsync(ToDoSubItemNotify item)
    {
        await ToDoService.AddFavoriteToDoItemAsync(item.Id).ConfigureAwait(false);
        await RefreshToDoItemAsync();
    }

    private Task ChangeToActiveDoItemAsync(ActiveToDoItemNotify item)
    {
        return ToDoService.NavigateToToDoItemViewModel(item.Id, Navigator);
    }

    public async Task UpdateItemsAsync(IEnumerable<ToDoSubItemNotify> items, IRefreshToDoItem refresh)
    {
        var itemsArray = items.ToArray();
        refreshToDoItem = refresh;

        await Dispatcher.UIThread.InvokeAsync(
            () =>
            {
                Missed.Clear();
                ReadyForCompleted.Clear();
                Completed.Clear();
                Planned.Clear();

                Missed.AddRange(itemsArray.Where(x => x.Status == ToDoItemStatus.Miss));
                ReadyForCompleted.AddRange(itemsArray.Where(x => x.Status == ToDoItemStatus.ReadyForComplete));
                Completed.AddRange(itemsArray.Where(x => x.Status == ToDoItemStatus.Completed));
                Planned.AddRange(itemsArray.Where(x => x.Status == ToDoItemStatus.Planned));
            }
        );

        await InitializedAsync();
    }
}