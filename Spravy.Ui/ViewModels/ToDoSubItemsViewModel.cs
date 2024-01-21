using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Ninject;
using Spravy.Domain.Extensions;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.Ui.Features.ToDo.ViewModels;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;
using Spravy.Ui.Services;

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
            var itemNotify = Mapper.Map<IEnumerable<ToDoItemNotify>>(item).Select(SetupItem);
            List.AddFavoritesAsync(itemNotify);
            cancellationToken.ThrowIfCancellationRequested();
        }
    }

    private ToDoItemNotify SetupItem(ToDoItemNotify item)
    {
        var toFavoriteCommand = CommandStorage.AddToDoItemToFavoriteItem.WithParam(item.Id);
        item.Commands.Add(CommandStorage.AddToDoItemChildItem.WithParam(item.Id));
        item.Commands.Add(CommandStorage.DeleteToDoItemItem.WithParam(item));

        if (item.IsCan != ToDoItemIsCan.None)
        {
            item.Commands.Add(CommandStorage.CompleteToDoItemItem.WithParam(item));
        }

        if (item.Type != ToDoItemType.Group)
        {
            item.Commands.Add(CommandStorage.ShowToDoSettingItem.WithParam(item));
        }

        if (item.IsFavorite)
        {
            toFavoriteCommand = CommandStorage.RemoveToDoItemFromFavoriteItem.WithParam(item.Id);
        }

        item.Commands.Add(toFavoriteCommand);
        item.Commands.Add(CommandStorage.NavigateToLeafItem.WithParam(item.Id));
        item.Commands.Add(CommandStorage.SetToDoParentItemItem.WithParam(item));
        item.Commands.Add(CommandStorage.MoveToDoItemToRootItem.WithParam(item));
        item.Commands.Add(CommandStorage.ToDoItemToStringItem.WithParam(item));
        item.Commands.Add(CommandStorage.ToDoItemRandomizeChildrenOrderIndexItem.WithParam(item));
        item.Commands.Add(CommandStorage.ChangeOrderIndexItem.WithParam(item));
        item.Commands.Add(CommandStorage.ResetToDoItemItem.WithParam(item));

        return item;
    }

    private async Task RefreshToDoItemListsAsync(Guid[] ids, CancellationToken cancellationToken)
    {
        await List.ClearAsync();
        cancellationToken.ThrowIfCancellationRequested();
        await RefreshFavoriteToDoItemsAsync(cancellationToken).ConfigureAwait(false);

        await foreach (var item in ToDoService.GetToDoItemsAsync(ids, 5, cancellationToken).ConfigureAwait(false))
        {
            var itemNotify = Mapper.Map<IEnumerable<ToDoItemNotify>>(item).Select(SetupItem);
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