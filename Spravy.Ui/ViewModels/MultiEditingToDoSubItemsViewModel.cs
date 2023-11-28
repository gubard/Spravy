using System;
using System.Collections.Generic;
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
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class MultiEditingToDoSubItemsViewModel : RoutableViewModelBase
{
    public MultiEditingToDoSubItemsViewModel() : base("multi-editing-to-do-sub-items")
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(RefreshAsync).RunAsync);
    }

    public AvaloniaList<Guid> Ids { get; } = new();
    public AvaloniaList<Selected<ToDoItemNotify>> Items { get; } = new();
    public ICommand InitializedCommand { get; }

    [Inject]
    public required IToDoService ToDoService { get; init; }

    [Inject]
    public required IMapper Mapper { get; init; }

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