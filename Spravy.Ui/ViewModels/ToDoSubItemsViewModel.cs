using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
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
    private readonly MultiToDoItemsViewModel list;

    [Inject]
    public required MultiToDoItemsViewModel List
    {
        get => list;
        [MemberNotNull(nameof(list))]
        init
        {
            list?.Dispose();
            list = value;
            Disposables.Add(list);
        }
    }

    [Inject]
    public required IToDoService ToDoService { get; init; }

    public ConfiguredValueTaskAwaitable<Result> RefreshAsync(CancellationToken cancellationToken)
    {
        return refreshToDoItem.ThrowIfNull().RefreshAsync(cancellationToken);
    }

    private ConfiguredValueTaskAwaitable<Result> RefreshFavoriteToDoItemsAsync(CancellationToken cancellationToken)
    {
        return ToDoService.GetFavoriteToDoItemIdsAsync(cancellationToken)
            .IfSuccessAsync(
                ids => List.ClearFavoriteExceptAsync(ids.ToArray())
                    .IfSuccessAsync(
                        () => RefreshFavoriteToDoItemsCore(ids, cancellationToken).ConfigureAwait(false),
                        cancellationToken
                    ),
                cancellationToken
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

    private ConfiguredValueTaskAwaitable<Result> RefreshToDoItemListsAsync(
        Guid[] ids,
        bool autoOrder,
        CancellationToken cancellationToken
    )
    {
        return List.ClearExceptAsync(ids)
            .IfSuccessAsync(() => RefreshFavoriteToDoItemsAsync(cancellationToken), cancellationToken)
            .IfSuccessAsync(
                () => RefreshToDoItemListsCore(ids, autoOrder, cancellationToken).ConfigureAwait(false),
                cancellationToken
            );
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

    public ConfiguredValueTaskAwaitable<Result> UpdateItemsAsync(
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