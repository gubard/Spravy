using Spravy.ToDo.Domain.Mapper.Mappers;
using Spravy.ToDo.Protos;

namespace Spravy.Router.Service.Services;

[Authorize]
public class GrpcRouterToDoService : ToDoService.ToDoServiceBase
{
    private readonly ISerializer serializer;
    private readonly IToDoService toDoService;

    public GrpcRouterToDoService(IToDoService toDoService, ISerializer serializer)
    {
        this.toDoService = toDoService;
        this.serializer = serializer;
    }

    public override Task<UpdateToDoItemOrderIndexReply> UpdateToDoItemOrderIndex(UpdateToDoItemOrderIndexRequest request, ServerCallContext context)
    {
        return toDoService.UpdateToDoItemOrderIndexAsync(request.Items.Select(x => x.ToUpdateOrderIndexToDoItemOptions()).ToArray(), context.CancellationToken).HandleAsync<UpdateToDoItemOrderIndexReply>(serializer, context.CancellationToken);
    }

    public override Task<GetCurrentActiveToDoItemReply> GetCurrentActiveToDoItem(GetCurrentActiveToDoItemRequest request, ServerCallContext context)
    {
        return toDoService.GetCurrentActiveToDoItemAsync(context.CancellationToken)
           .HandleAsync(
                serializer,
                active => new GetCurrentActiveToDoItemReply
                {
                    Item = active.ToToDoShortItemNullableGrpc()
                },
                context.CancellationToken
            );
    }

    public override Task<GetChildrenToDoItemIdsReply> GetChildrenToDoItemIds(GetChildrenToDoItemIdsRequest request, ServerCallContext context)
    {
        return toDoService.GetChildrenToDoItemIdsAsync(request.Id.ToOptionGuid(), request.IgnoreIds.ToGuid(), context.CancellationToken)
           .HandleAsync(
                serializer,
                ids =>
                {
                    var result = new GetChildrenToDoItemIdsReply();
                    result.Ids.AddRange(ids.ToByteString().ToArray());

                    return result;
                },
                context.CancellationToken
            );
    }

    public override Task<ToDoItemToStringReply> ToDoItemToString(ToDoItemToStringRequest request, ServerCallContext context)
    {
        return toDoService.ToDoItemToStringAsync(request.Items.Select(x => x.ToToDoItemToStringOptions()).ToArray(), context.CancellationToken)
           .HandleAsync(
                serializer,
                value => new ToDoItemToStringReply
                {
                    Value = value
                },
                context.CancellationToken
            );
    }

    public override Task<GetShortToDoItemsReply> GetShortToDoItems(GetShortToDoItemsRequest request, ServerCallContext context)
    {
        return toDoService.GetShortToDoItemsAsync(request.Ids.ToGuid(), context.CancellationToken)
           .HandleAsync(
                serializer,
                items =>
                {
                    var result = new GetShortToDoItemsReply();
                    result.Items.AddRange(items.Select(x => x.ToToDoShortItemGrpc()).ToArray());

                    return result;
                },
                context.CancellationToken
            );
    }

    public override Task<GetActiveToDoItemReply> GetActiveToDoItem(GetActiveToDoItemRequest request, ServerCallContext context)
    {
        return toDoService.GetActiveToDoItemAsync(request.Id.ToGuid(), context.CancellationToken)
           .HandleAsync(
                serializer,
                activeToDoItem => new GetActiveToDoItemReply
                {
                    Item = activeToDoItem.ToToDoShortItemNullableGrpc()
                },
                context.CancellationToken
            );
    }

    public override Task<ResetToDoItemReply> ResetToDoItem(ResetToDoItemRequest request, ServerCallContext context)
    {
        return toDoService.ResetToDoItemAsync(request.Items.Select(x=>x.ToResetToDoItemOptions()).ToArray(), context.CancellationToken).HandleAsync<ResetToDoItemReply>(serializer, context.CancellationToken);
    }

    public override Task<RandomizeChildrenOrderIndexReply> RandomizeChildrenOrderIndex(RandomizeChildrenOrderIndexRequest request, ServerCallContext context)
    {
        return toDoService.RandomizeChildrenOrderIndexAsync(request.Ids.ToGuid(), context.CancellationToken).HandleAsync<RandomizeChildrenOrderIndexReply>(serializer, context.CancellationToken);
    }

    public override Task<EditToDoItemsReply> EditToDoItems(EditToDoItemsRequest request, ServerCallContext context)
    {
        return toDoService.EditToDoItemsAsync(request.Value.ToEditToDoItems(), context.CancellationToken).HandleAsync<EditToDoItemsReply>(serializer, context.CancellationToken);
    }

    public override Task<DeleteToDoItemsReply> DeleteToDoItems(DeleteToDoItemsRequest request, ServerCallContext context)
    {
        return toDoService.DeleteToDoItemsAsync(request.Ids.ToGuid(), context.CancellationToken).HandleAsync<DeleteToDoItemsReply>(serializer, context.CancellationToken);
    }

    public override Task<CloneToDoItemReply> CloneToDoItem(CloneToDoItemRequest request, ServerCallContext context)
    {
        return toDoService.CloneToDoItemAsync(request.CloneIds.ToGuid(),request.ParentId.ToOptionGuid(), context.CancellationToken)
           .HandleAsync(
                serializer,
                ids =>
                {
                    var result = new CloneToDoItemReply();
                    result.NewItemIds.AddRange(ids.ToByteString().ToArray());

                    return result;
                },
                context.CancellationToken
            );
    }

    public override Task<AddToDoItemReply> AddToDoItem(AddToDoItemRequest request, ServerCallContext context)
    {
        return toDoService.AddToDoItemAsync(request.Items.Select(x=>x.ToAddToDoItemOptions()).ToArray(), context.CancellationToken)
           .HandleAsync(
                serializer,
                ids =>
                {
                    var result = new AddToDoItemReply();
                    result.Ids.AddRange(ids.ToByteString().ToArray());

                    return result;
                },
                context.CancellationToken
            );
    }

    public override Task<UpdateEventsReply> UpdateEvents(UpdateEventsRequest request, ServerCallContext context)
    {
        return toDoService.UpdateEventsAsync(context.CancellationToken)
           .HandleAsync(
                serializer,
                isUpdated => new UpdateEventsReply
                {
                    IsUpdated = isUpdated,
                },
                context.CancellationToken
            );
    }

    public override Task<GetBookmarkToDoItemIdsRequestReply> GetBookmarkToDoItemIds(GetBookmarkToDoItemIdsRequest request, ServerCallContext context)
    {
        return toDoService.GetBookmarkToDoItemIdsAsync(context.CancellationToken)
           .HandleAsync(
                serializer,
                ids =>
                {
                    var result = new GetBookmarkToDoItemIdsRequestReply();
                    result.Ids.AddRange(ids.ToByteString().ToArray());

                    return result;
                },
                context.CancellationToken
            );
    }

    public override Task<SwitchCompleteReply> SwitchComplete(SwitchCompleteRequest request, ServerCallContext context)
    {
        return toDoService.SwitchCompleteAsync(request.Ids.ToGuid(), context.CancellationToken).HandleAsync<SwitchCompleteReply>(serializer, context.CancellationToken);
    }

    public override Task<GetTodayToDoItemsReply> GetTodayToDoItems(
        GetTodayToDoItemsRequest request,
        ServerCallContext context
    )
    {
        return toDoService.GetTodayToDoItemsAsync(context.CancellationToken)
           .HandleAsync(
                serializer,
                ids =>
                {
                    var result = new GetTodayToDoItemsReply();
                    result.Ids.AddRange(ids.ToByteString().ToArray());

                    return result;
                },
                context.CancellationToken
            );
    }

    public override Task<GetLeafToDoItemIdsReply> GetLeafToDoItemIds(
        GetLeafToDoItemIdsRequest request,
        ServerCallContext context
    )
    {
        return toDoService.GetLeafToDoItemIdsAsync(request.Id.ToGuid(), context.CancellationToken)
           .HandleAsync(
                serializer,
                ids =>
                {
                    var reply = new GetLeafToDoItemIdsReply();
                    reply.Ids.AddRange(ids.ToByteString().ToArray());

                    return reply;
                },
                context.CancellationToken
            );
    }

    public override Task<GetFavoriteToDoItemIdsRequestReply> GetFavoriteToDoItemIds(
        GetFavoriteToDoItemIdsRequest request,
        ServerCallContext context
    )
    {
        return toDoService.GetFavoriteToDoItemIdsAsync(context.CancellationToken)
           .HandleAsync(
                serializer,
                ids =>
                {
                    var reply = new GetFavoriteToDoItemIdsRequestReply();
                    reply.Ids.AddRange(ids.ToByteString().ToArray());

                    return reply;
                },
                context.CancellationToken
            );
    }

    public override Task<SearchToDoItemIdsReply> SearchToDoItemIds(
        SearchToDoItemIdsRequest request,
        ServerCallContext context
    )
    {
        return toDoService.SearchToDoItemIdsAsync(request.SearchText, context.CancellationToken)
           .HandleAsync(
                serializer,
                ids =>
                {
                    var reply = new SearchToDoItemIdsReply();
                    reply.Ids.AddRange(ids.ToByteString().ToArray());

                    return reply;
                },
                context.CancellationToken
            );
    }

    public override async Task GetToDoItems(
        GetToDoItemsRequest request,
        IServerStreamWriter<GetToDoItemsReply> responseStream,
        ServerCallContext context
    )
    {
        var ids = request.Ids.ToGuid();

        await foreach (var item in toDoService.GetToDoItemsAsync(ids, request.ChunkSize, context.CancellationToken))
        {
            var reply = new GetToDoItemsReply();
            reply.Items.AddRange(item.ThrowIfError().ToFullToDoItemGrpc().ToArray());
            await responseStream.WriteAsync(reply);
        }
    }

    public override Task<GetToDoItemReply> GetToDoItem(GetToDoItemRequest request, ServerCallContext context)
    {
        return toDoService.GetToDoItemAsync(request.Id.ToGuid(), context.CancellationToken)
           .HandleAsync(
                serializer,
                item =>
                {
                    var reply = new GetToDoItemReply
                    {
                        Item = item.ToFullToDoItemGrpc(),
                    };

                    return reply;
                },
                context.CancellationToken
            );
    }

    public override Task<GetParentsReply> GetParents(GetParentsRequest request, ServerCallContext context)
    {
        return toDoService.GetParentsAsync(request.Id.ToGuid(), context.CancellationToken)
           .HandleAsync(
                serializer,
                parents =>
                {
                    var reply = new GetParentsReply();
                    reply.Parents.AddRange(parents.ToToDoShortItemGrpc().ToArray());

                    return reply;
                },
                context.CancellationToken
            );
    }

    public override Task<GetToDoSelectorItemsReply> GetToDoSelectorItems(
        GetToDoSelectorItemsRequest request,
        ServerCallContext context
    )
    {
        return toDoService.GetToDoSelectorItemsAsync(request.IgnoreIds.ToGuid(), context.CancellationToken)
           .HandleAsync(
                serializer,
                items =>
                {
                    var reply = new GetToDoSelectorItemsReply();
                    reply.Items.AddRange(items.ToToDoSelectorItemGrpc().ToArray());

                    return reply;
                },
                context.CancellationToken
            );
    }
}