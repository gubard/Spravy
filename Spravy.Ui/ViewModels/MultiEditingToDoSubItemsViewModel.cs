using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Avalonia.Collections;
using Avalonia.Threading;
using Ninject;
using ReactiveUI;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.ToDo.Domain.Models;
using Spravy.Ui.Enums;
using Spravy.Ui.Extensions;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class MultiEditingToDoSubItemsViewModel : RoutableViewModelBase
{
    private MultiEditingGroupBy groupBy;

    public MultiEditingToDoSubItemsViewModel() : base("multi-editing-to-do-sub-items")
    {
        SwitchPaneCommand = CreateCommand(SwitchPane);
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(RefreshAsync).RunAsync);
        CompleteCommand = CreateInitializedCommand(TaskWork.Create(CompleteAsync).RunAsync);
        ChangeTypeCommand = CreateInitializedCommand(TaskWork.Create(ChangeTypeAsync).RunAsync);
        SelectAllCommand = CreateInitializedCommand(TaskWork.Create(SelectAllAsync).RunAsync);
        GroupBys = new(Enum.GetValues<MultiEditingGroupBy>());
        SelectAllMissedCommand = CreateInitializedCommand(TaskWork.Create(SelectAllMissedAsync).RunAsync);
        SelectAllPlannedCommand = CreateInitializedCommand(TaskWork.Create(SelectAllPlannedAsync).RunAsync);
        SelectAllReadyForCompletedCommand =
            CreateInitializedCommand(TaskWork.Create(SelectAllReadyForCompletedAsync).RunAsync);
        SelectAllCompletedCommand = CreateInitializedCommand(TaskWork.Create(SelectAllCompletedAsync).RunAsync);
        SelectAllPlannedsCommand = CreateInitializedCommand(TaskWork.Create(SelectAllPlannedsAsync).RunAsync);
        SelectAllPeriodicitysCommand = CreateInitializedCommand(TaskWork.Create(SelectAllPeriodicitysAsync).RunAsync);
        SelectAllPeriodicityOffsetsCommand =
            CreateInitializedCommand(TaskWork.Create(SelectAllPeriodicityOffsetsAsync).RunAsync);
        SelectAllCirclesCommand = CreateInitializedCommand(TaskWork.Create(SelectAllCirclesAsync).RunAsync);
        SelectAllStepsCommand = CreateInitializedCommand(TaskWork.Create(SelectAllStepsAsync).RunAsync);
        SelectAllValuesCommand = CreateInitializedCommand(TaskWork.Create(SelectAllValuesAsync).RunAsync);
        SelectAllGroupsCommand = CreateInitializedCommand(TaskWork.Create(SelectAllGroupsAsync).RunAsync);
        ChangeRootItemCommand =  CreateInitializedCommand(TaskWork.Create(ChangeRootItemAsync).RunAsync);
    }

    public AvaloniaList<Guid> Ids { get; } = new();
    public AvaloniaList<Selected<ToDoItemNotify>> Items { get; } = new();
    public AvaloniaList<Selected<ToDoItemNotify>> Missed { get; } = new();
    public AvaloniaList<Selected<ToDoItemNotify>> Planned { get; } = new();
    public AvaloniaList<Selected<ToDoItemNotify>> ReadyForCompleted { get; } = new();
    public AvaloniaList<Selected<ToDoItemNotify>> Completed { get; } = new();
    public AvaloniaList<Selected<ToDoItemNotify>> Values { get; } = new();
    public AvaloniaList<Selected<ToDoItemNotify>> Groups { get; } = new();
    public AvaloniaList<Selected<ToDoItemNotify>> Planneds { get; } = new();
    public AvaloniaList<Selected<ToDoItemNotify>> Periodicitys { get; } = new();
    public AvaloniaList<Selected<ToDoItemNotify>> PeriodicityOffsets { get; } = new();
    public AvaloniaList<Selected<ToDoItemNotify>> Circles { get; } = new();
    public AvaloniaList<Selected<ToDoItemNotify>> Steps { get; } = new();
    public AvaloniaList<MultiEditingGroupBy> GroupBys { get; }
    public ICommand InitializedCommand { get; }
    public ICommand SwitchPaneCommand { get; }
    public ICommand CompleteCommand { get; }
    public ICommand ChangeTypeCommand { get; }
    public ICommand SelectAllCommand { get; }
    public ICommand SelectAllMissedCommand { get; }
    public ICommand SelectAllPlannedCommand { get; }
    public ICommand SelectAllReadyForCompletedCommand { get; }
    public ICommand SelectAllCompletedCommand { get; }
    public ICommand SelectAllValuesCommand { get; }
    public ICommand SelectAllGroupsCommand { get; }
    public ICommand SelectAllPlannedsCommand { get; }
    public ICommand SelectAllPeriodicitysCommand { get; }
    public ICommand SelectAllPeriodicityOffsetsCommand { get; }
    public ICommand SelectAllCirclesCommand { get; }
    public ICommand SelectAllStepsCommand { get; }
    public ICommand ChangeRootItemCommand { get; }

    [Inject]
    public required IToDoService ToDoService { get; init; }

    [Inject]
    public required IMapper Mapper { get; init; }

    [Inject]
    public required MainSplitViewModel MainSplitViewModel { get; init; }

    public MultiEditingGroupBy GroupBy
    {
        get => groupBy;
        set => this.RaiseAndSetIfChanged(ref groupBy, value);
    }

    private Task SelectAllStepsAsync(CancellationToken arg)
    {
        return Select(Steps);
    }

    private Task SelectAllCirclesAsync(CancellationToken arg)
    {
        return Select(Circles);
    }

    private Task SelectAllPeriodicityOffsetsAsync(CancellationToken arg)
    {
        return Select(PeriodicityOffsets);
    }

    private Task SelectAllPeriodicitysAsync(CancellationToken arg)
    {
        return Select(Periodicitys);
    }

    private Task SelectAllPlannedsAsync(CancellationToken arg)
    {
        return Select(Planneds);
    }

    private Task SelectAllGroupsAsync(CancellationToken arg)
    {
        return Select(Groups);
    }

    private Task SelectAllValuesAsync(CancellationToken arg)
    {
        return Select(Values);
    }

    private Task SelectAllCompletedAsync(CancellationToken arg)
    {
        return Select(Completed);
    }

    private Task SelectAllReadyForCompletedAsync(CancellationToken arg)
    {
        return Select(ReadyForCompleted);
    }

    private Task SelectAllPlannedAsync(CancellationToken arg)
    {
        return Select(Planned);
    }

    private Task SelectAllMissedAsync(CancellationToken arg)
    {
        return Select(Missed);
    }

    private Task SelectAllAsync(CancellationToken arg)
    {
        return Select(Items);
    }

    private async Task Select(AvaloniaList<Selected<ToDoItemNotify>> items)
    {
        await Dispatcher.UIThread.InvokeAsync(
            () =>
            {
                if (items.All(x => x.IsSelect))
                {
                    foreach (var item in items)
                    {
                        item.IsSelect = false;
                    }
                }
                else
                {
                    foreach (var item in items)
                    {
                        item.IsSelect = true;
                    }
                }
            }
        );
    }

    private async Task ChangeRootItemAsync(CancellationToken cancellationToken)
    {
        var ids = Items.Where(x => x.IsSelect).Select(x => x.Value.Id).ToArray();

        await DialogViewer.ShowToDoItemSelectorConfirmDialogAsync(
                async item =>
                {
                    await DialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);

                    foreach (var id in ids)
                    {
                        await ToDoService.UpdateToDoItemParentAsync(id, item.Id, cancellationToken)
                            .ConfigureAwait(false);
                    }

                    await RefreshAsync(cancellationToken).ConfigureAwait(false);
                },
                viewModel => viewModel.IgnoreIds.AddRange(ids),
                cancellationToken
            )
            .ConfigureAwait(false);
    }

    private DispatcherOperation SwitchPane()
    {
        return Dispatcher.UIThread.InvokeAsync(() => MainSplitViewModel.IsPaneOpen = !MainSplitViewModel.IsPaneOpen);
    }

    public async Task CompleteAsync(CancellationToken cancellationToken)
    {
        var items = Items.Where(x => x.IsSelect).Select(x => x.Value).ToArray();

        await DialogViewer.ShowInfoInputDialogAsync<CompleteToDoItemViewModel>(
            _ => DialogViewer.CloseInputDialogAsync(cancellationToken),
            viewModel =>
            {
                viewModel.SetAllStatus();

                viewModel.Complete = async status =>
                {
                    await DialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);
                    await CompleteAsync(items, status, cancellationToken).ConfigureAwait(false);
                    await RefreshAsync(cancellationToken).ConfigureAwait(false);
                };
            },
            cancellationToken
        );
    }

    private async Task ChangeTypeAsync(CancellationToken cancellationToken)
    {
        await DialogViewer.ShowItemSelectorDialogAsync<ToDoItemType>(
                async type =>
                {
                    await DialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);

                    foreach (var item in Items)
                    {
                        if (!item.IsSelect)
                        {
                            continue;
                        }

                        await ToDoService.UpdateToDoItemTypeAsync(item.Value.Id, type, cancellationToken)
                            .ConfigureAwait(false);
                    }

                    await RefreshAsync(cancellationToken);
                },
                viewModel =>
                {
                    viewModel.Items.AddRange(Enum.GetValues<ToDoItemType>().OfType<object>());
                    viewModel.SelectedItem = viewModel.Items.First();
                },
                cancellationToken
            )
            .ConfigureAwait(false);
    }

    private async Task CompleteAsync(
        IEnumerable<ToDoItemNotify> items,
        CompleteStatus status,
        CancellationToken cancellationToken
    )
    {
        switch (status)
        {
            case CompleteStatus.Complete:
                foreach (var item in items)
                {
                    await ToDoService.UpdateToDoItemCompleteStatusAsync(item.Id, true, cancellationToken)
                        .ConfigureAwait(false);
                }

                break;
            case CompleteStatus.Skip:
                foreach (var item in items)
                {
                    await ToDoService.SkipToDoItemAsync(item.Id, cancellationToken).ConfigureAwait(false);
                }

                break;
            case CompleteStatus.Fail:
                foreach (var item in items)
                {
                    await ToDoService.FailToDoItemAsync(item.Id, cancellationToken).ConfigureAwait(false);
                }

                break;
            case CompleteStatus.Incomplete:
                foreach (var item in items)
                {
                    await ToDoService.UpdateToDoItemCompleteStatusAsync(item.Id, true, cancellationToken)
                        .ConfigureAwait(false);
                }

                break;
            default: throw new ArgumentOutOfRangeException(nameof(status), status, null);
        }
    }

    public async Task RefreshAsync(CancellationToken cancellationToken)
    {
        await Dispatcher.UIThread.InvokeAsync(
            () =>
            {
                Items.Clear();
                Missed.Clear();
                Planned.Clear();
                ReadyForCompleted.Clear();
                Completed.Clear();
                Values.Clear();
                Groups.Clear();
                Planneds.Clear();
                Periodicitys.Clear();
                PeriodicityOffsets.Clear();
                Circles.Clear();
                Steps.Clear();
            }
        );

        await foreach (var item in ToDoService.GetToDoItemsAsync(Ids.ToArray(), cancellationToken).ConfigureAwait(false))
        {
            await AddToDoItemAsync(item);
        }
    }

    private DispatcherOperation AddToDoItemAsync(ToDoItem item)
    {
        var itemNotify = Mapper.Map<ToDoItemNotify>(item);

        return AddItem(itemNotify);
    }

    private DispatcherOperation AddItem(ToDoItemNotify item)
    {
        var selected = new Selected<ToDoItemNotify>(item);

        return Dispatcher.UIThread.InvokeAsync(
            () =>
            {
                Items.Add(selected);

                switch (item.Status)
                {
                    case ToDoItemStatus.Miss:
                        Missed.Add(selected);
                        break;
                    case ToDoItemStatus.ReadyForComplete:
                        ReadyForCompleted.Add(selected);
                        break;
                    case ToDoItemStatus.Planned:
                        Planned.Add(selected);
                        break;
                    case ToDoItemStatus.Completed:
                        Completed.Add(selected);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                switch (item.Type)
                {
                    case ToDoItemType.Value:
                        Values.Add(selected);
                        break;
                    case ToDoItemType.Group:
                        Groups.Add(selected);
                        break;
                    case ToDoItemType.Planned:
                        Planneds.Add(selected);
                        break;
                    case ToDoItemType.Periodicity:
                        Periodicitys.Add(selected);
                        break;
                    case ToDoItemType.PeriodicityOffset:
                        PeriodicityOffsets.Add(selected);
                        break;
                    case ToDoItemType.Circle:
                        Circles.Add(selected);
                        break;
                    case ToDoItemType.Step:
                        Steps.Add(selected);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        );
    }
}