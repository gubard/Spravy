using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Avalonia.Collections;
using Avalonia.Threading;
using Ninject;
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
    public MultiEditingToDoSubItemsViewModel() : base("multi-editing-to-do-sub-items")
    {
        SwitchPaneCommand = CreateCommand(SwitchPane);
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(RefreshAsync).RunAsync);
        CompleteCommand = CreateInitializedCommand(TaskWork.Create(CompleteAsync).RunAsync);
        ChangeTypeCommand = CreateInitializedCommand(TaskWork.Create(ChangeTypeAsync).RunAsync);
        SelectAllCommand = CreateInitializedCommand(TaskWork.Create(SelectAllAsync).RunAsync);
    }

    private async Task SelectAllAsync(CancellationToken arg)
    {
        if (Items.All(x => x.IsSelect))
        {
            foreach (var item in Items)
            {
                await Dispatcher.UIThread.InvokeAsync(() => item.IsSelect = false);
            }
        }
        else
        {
            foreach (var item in Items)
            {
                await Dispatcher.UIThread.InvokeAsync(() => item.IsSelect = true);
            }
        }
    }

    public AvaloniaList<Guid> Ids { get; } = new();
    public AvaloniaList<Selected<ToDoItemNotify>> Items { get; } = new();
    public ICommand InitializedCommand { get; }
    public ICommand SwitchPaneCommand { get; }
    public ICommand CompleteCommand { get; }
    public ICommand ChangeTypeCommand { get; }
    public ICommand SelectAllCommand { get; }

    [Inject]
    public required IToDoService ToDoService { get; init; }

    [Inject]
    public required IMapper Mapper { get; init; }

    [Inject]
    public required MainSplitViewModel MainSplitViewModel { get; init; }

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
            () => { Items.Clear(); }
        );

        await foreach (var item in LoadToDoItemsAsync(Ids, cancellationToken).ConfigureAwait(false))
        {
            await AddToDoItemAsync(item);
        }
    }

    private async IAsyncEnumerable<ToDoItem> LoadToDoItemsAsync(
        IEnumerable<Guid> ids,
        [EnumeratorCancellation] CancellationToken cancellationToken
    )
    {
        foreach (var id in ids)
        {
            yield return await ToDoService.GetToDoItemAsync(id, cancellationToken).ConfigureAwait(false);
        }
    }

    private DispatcherOperation AddToDoItemAsync(ToDoItem item)
    {
        var itemNotify = Mapper.Map<ToDoItemNotify>(item);

        return Dispatcher.UIThread.InvokeAsync(() => Items.Add(new Selected<ToDoItemNotify>(itemNotify)));
    }
}