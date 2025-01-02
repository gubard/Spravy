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

    public override Task<GetReply> Get(GetRequest request, ServerCallContext context)
    {
        return toDoService.GetAsync(request.ToGetToDo(), context.CancellationToken).HandleAsync(serializer, response => response.ToGetReply(), context.CancellationToken);
    }

    public override Task<UpdateToDoItemOrderIndexReply> UpdateToDoItemOrderIndex(UpdateToDoItemOrderIndexRequest request, ServerCallContext context)
    {
        return toDoService.UpdateToDoItemOrderIndexAsync(request.Items.Select(x => x.ToUpdateOrderIndexToDoItemOptions()).ToArray(), context.CancellationToken).HandleAsync<UpdateToDoItemOrderIndexReply>(serializer, context.CancellationToken);
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

    public override Task<SwitchCompleteReply> SwitchComplete(SwitchCompleteRequest request, ServerCallContext context)
    {
        return toDoService.SwitchCompleteAsync(request.Ids.ToGuid(), context.CancellationToken).HandleAsync<SwitchCompleteReply>(serializer, context.CancellationToken);
    }
}