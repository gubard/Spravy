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