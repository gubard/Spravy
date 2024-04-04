using System;
using System.Threading;
using System.Threading.Tasks;
using Ninject;
using Spravy.Domain.Extensions;
using Spravy.Domain.Models;
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

    public async ValueTask<Result> RefreshAsync(CancellationToken cancellationToken)
    {
        await refreshToDoItem.ThrowIfNull().RefreshAsync(cancellationToken).ConfigureAwait(false);

        return Result.Success;
    }

    private ValueTask<Result> RefreshFavoriteToDoItemsAsync(CancellationToken cancellationToken)
    {
        return ToDoService.GetFavoriteToDoItemIdsAsync(cancellationToken)
            .ConfigureAwait(false)
            .IfSuccessAsync(
                ids => List.ClearFavoriteExceptAsync(ids.ToArray())
                    .ConfigureAwait(false)
                    .IfSuccessAsync(
                        () => RefreshFavoriteToDoItemsCore(ids, cancellationToken).ConfigureAwait(false)
                    )
                    .ConfigureAwait(false)
            );
    }

    private async ValueTask<Result> RefreshFavoriteToDoItemsCore(
        ReadOnlyMemory<Guid> ids,
        CancellationToken cancellationToken
    )
    {
        await foreach (var items in ToDoService
                           .GetToDoItemsAsync(ids.ToArray(), 5, cancellationToken)
                           .ConfigureAwait(false))
        {
            foreach (var item in items.ToArray())
            {
                var result = await List.UpdateFavoriteItemAsync(item);

                if (result.IsHasError)
                {
                    return result;
                }
            }
        }

        return Result.Success;
    }

    private ValueTask<Result> RefreshToDoItemListsAsync(
        Guid[] ids,
        bool autoOrder,
        CancellationToken cancellationToken
    )
    {
        return List.ClearExceptAsync(ids)
            .ConfigureAwait(false)
            .IfSuccessAsync(() => RefreshFavoriteToDoItemsAsync(cancellationToken).ConfigureAwait(false))
            .ConfigureAwait(false)
            .IfSuccessAsync(() => RefreshToDoItemListsCore(ids, autoOrder, cancellationToken).ConfigureAwait(false));
    }

    private async ValueTask<Result> RefreshToDoItemListsCore(
        Guid[] ids,
        bool autoOrder,
        CancellationToken cancellationToken
    )
    {
        uint orderIndex = 1;

        await foreach (var items in ToDoService.GetToDoItemsAsync(ids, 5, cancellationToken).ConfigureAwait(false))
        {
            foreach (var item in items.ToArray())
            {
                if (autoOrder)
                {
                    var result = await List.UpdateItemAsync(item.WithOrderIndex(orderIndex));

                    if (result.IsHasError)
                    {
                        return result;
                    }
                }
                else
                {
                    var result = await List.UpdateItemAsync(item);

                    if (result.IsHasError)
                    {
                        return result;
                    }
                }

                orderIndex++;
            }
        }

        return Result.Success;
    }

    public ValueTask<Result> UpdateItemsAsync(
        Guid[] ids,
        IRefresh refresh,
        bool autoOrder,
        CancellationToken cancellationToken
    )
    {
        refreshToDoItem = refresh;

        return RefreshToDoItemListsAsync(ids, autoOrder, cancellationToken);
    }
}