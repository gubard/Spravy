using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Ninject;
using Spravy.Domain.Extensions;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.Ui.Features.ToDo.ViewModels;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class ToDoSubItemsViewModel : ViewModelBase, IToDoItemOrderChanger
{
    private IRefresh? refreshToDoItem;

    [Inject]
    public required MultiToDoItemsViewModel List { get; init; }

    [Inject]
    public required IToDoService ToDoService { get; init; }

    [Inject]
    public required IMapper Mapper { get; init; }

    public async Task RefreshAsync(CancellationToken cancellationToken)
    {
        await refreshToDoItem.ThrowIfNull().RefreshAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task RefreshFavoriteToDoItemsAsync(CancellationToken cancellationToken)
    {
        var ids = await ToDoService.GetFavoriteToDoItemIdsAsync(cancellationToken).ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();

        await foreach (var item in ToDoService.GetToDoItemsAsync(ids.ToArray(), 5, cancellationToken)
                           .ConfigureAwait(false))
        {
            var itemNotify = Mapper.Map<IEnumerable<ToDoItemNotify>>(item);
            List.AddFavoritesAsync(itemNotify);
            cancellationToken.ThrowIfCancellationRequested();
        }
    }

    private async Task RefreshToDoItemListsAsync(Guid[] ids, CancellationToken cancellationToken)
    {
        await List.ClearAsync();
        cancellationToken.ThrowIfCancellationRequested();
        await RefreshFavoriteToDoItemsAsync(cancellationToken).ConfigureAwait(false);

        await foreach (var item in ToDoService.GetToDoItemsAsync(ids, 5, cancellationToken).ConfigureAwait(false))
        {
            var itemNotify = Mapper.Map<IEnumerable<ToDoItemNotify>>(item);
            await List.AddItemsAsync(itemNotify);
            cancellationToken.ThrowIfCancellationRequested();
        }
    }

    public async Task UpdateItemsAsync(Guid[] ids, IRefresh refresh, CancellationToken cancellationToken)
    {
        refreshToDoItem = refresh;
        await RefreshToDoItemListsAsync(ids, cancellationToken).ConfigureAwait(false);
    }
}