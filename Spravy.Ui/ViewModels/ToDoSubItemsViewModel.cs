using System;
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

    public async Task RefreshAsync(CancellationToken cancellationToken)
    {
        await refreshToDoItem.ThrowIfNull().RefreshAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task RefreshFavoriteToDoItemsAsync(CancellationToken cancellationToken)
    {
        var ids = (await ToDoService.GetFavoriteToDoItemIdsAsync(cancellationToken).ConfigureAwait(false)).ToArray();
        await List.ClearFavoriteExceptAsync(ids);
        cancellationToken.ThrowIfCancellationRequested();

        await foreach (var items in ToDoService.GetToDoItemsAsync(ids, 5, cancellationToken)
                           .ConfigureAwait(false))
        {
            foreach (var item in items)
            {
                await List.UpdateFavoriteItemAsync(item);
            }

            cancellationToken.ThrowIfCancellationRequested();
        }
    }

    private async Task RefreshToDoItemListsAsync(Guid[] ids, bool autoOrder, CancellationToken cancellationToken)
    {
        await List.ClearExceptAsync(ids);
        cancellationToken.ThrowIfCancellationRequested();
        await RefreshFavoriteToDoItemsAsync(cancellationToken).ConfigureAwait(false);
        uint orderIndex = 1;

        await foreach (var items in ToDoService.GetToDoItemsAsync(ids, 5, cancellationToken).ConfigureAwait(false))
        {
            foreach (var item in items)
            {
                if (autoOrder)
                {
                    await List.UpdateItemAsync(item.WithOrderIndex(orderIndex));
                }
                else
                {
                    await List.UpdateItemAsync(item);
                }

                orderIndex++;
            }

            cancellationToken.ThrowIfCancellationRequested();
        }
    }

    public async Task UpdateItemsAsync(
        Guid[] ids,
        IRefresh refresh,
        bool autoOrder,
        CancellationToken cancellationToken
    )
    {
        refreshToDoItem = refresh;
        await RefreshToDoItemListsAsync(ids, autoOrder, cancellationToken).ConfigureAwait(false);
    }
}