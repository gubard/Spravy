using AutoMapper;
using Google.Protobuf;
using Grpc.Core;
using Spravy.Domain.Enums;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.Protos;

namespace Spravy.Service.Services.Grpcs;

public class GrpcToDoService : ToDoService.ToDoServiceBase
{
    private readonly IToDoService toDoService;
    private readonly IMapper mapper;

    public GrpcToDoService(IToDoService toDoService, IMapper mapper)
    {
        this.toDoService = toDoService;
        this.mapper = mapper;
    }

    public override async Task<GetRootToDoItemsReply> GetRootToDoItems(
        GetRootToDoItemsRequest request,
        ServerCallContext context
    )
    {
        var reply = new GetRootToDoItemsReply();
        var items = await toDoService.GetRootToDoItemsAsync();
        reply.Items.AddRange(mapper.Map<IEnumerable<ToDoSubItemGrpc>>(items));

        return reply;
    }

    public override async Task<GetToDoItemReply> GetToDoItem(GetToDoItemRequest request, ServerCallContext context)
    {
        var item = await toDoService.GetToDoItemAsync(mapper.Map<Guid>(request.Id));

        var reply = new GetToDoItemReply
        {
            Item = mapper.Map<ToDoItemGrpc>(item),
        };

        return reply;
    }

    public override async Task<AddRootToDoItemReply> AddRootToDoItem(
        AddRootToDoItemRequest request,
        ServerCallContext context
    )
    {
        var id = await toDoService.AddRootToDoItemAsync(mapper.Map<AddRootToDoItemOptions>(request));

        var reply = new AddRootToDoItemReply
        {
            Id = mapper.Map<ByteString>(id)
        };

        return reply;
    }

    public override async Task<AddToDoItemReply> AddToDoItem(AddToDoItemRequest request, ServerCallContext context)
    {
        var id = await toDoService.AddToDoItemAsync(mapper.Map<AddToDoItemOptions>(request));

        var reply = new AddToDoItemReply
        {
            Id = mapper.Map<ByteString>(id),
        };

        return reply;
    }

    public override async Task<DeleteToDoItemReply> DeleteToDoItem(
        DeleteToDoItemRequest request,
        ServerCallContext context
    )
    {
        await toDoService.DeleteToDoItemAsync(mapper.Map<Guid>(request.Id));

        return new DeleteToDoItemReply();
    }

    public override async Task<UpdateTypeOfPeriodicityReply> UpdateTypeOfPeriodicity(
        UpdateTypeOfPeriodicityRequest request,
        ServerCallContext context
    )
    {
        await toDoService.UpdateTypeOfPeriodicityAsync(mapper.Map<Guid>(request.Id), (TypeOfPeriodicity)request.Type);

        return new UpdateTypeOfPeriodicityReply();
    }

    public override async Task<UpdateDueDateReply> UpdateDueDate(
        UpdateDueDateRequest request,
        ServerCallContext context
    )
    {
        var dueDate = mapper.Map<DateTimeOffset>(request.DueDate);
        await toDoService.UpdateDueDateAsync(mapper.Map<Guid>(request.Id), dueDate);

        return new UpdateDueDateReply();
    }

    public override async Task<UpdateCompleteStatusReply> UpdateCompleteStatus(
        UpdateCompleteStatusRequest request,
        ServerCallContext context
    )
    {
        await toDoService.UpdateCompleteStatusAsync(mapper.Map<Guid>(request.Id), request.IsCompleted);

        return new UpdateCompleteStatusReply();
    }

    public override async Task<UpdateNameToDoItemReply> UpdateNameToDoItem(
        UpdateNameToDoItemRequest request,
        ServerCallContext context
    )
    {
        await toDoService.UpdateNameToDoItemAsync(mapper.Map<Guid>(request.Id), request.Name);

        return new UpdateNameToDoItemReply();
    }

    public override async Task<UpdateOrderIndexToDoItemReply> UpdateOrderIndexToDoItem(
        UpdateOrderIndexToDoItemRequest request,
        ServerCallContext context
    )
    {
        var options = mapper.Map<UpdateOrderIndexToDoItemOptions>(request);
        await toDoService.UpdateOrderIndexToDoItemAsync(options);

        return new UpdateOrderIndexToDoItemReply();
    }

    public override async Task<UpdateDescriptionToDoItemReply> UpdateDescriptionToDoItem(
        UpdateDescriptionToDoItemRequest request,
        ServerCallContext context
    )
    {
        await toDoService.UpdateDescriptionToDoItemAsync(mapper.Map<Guid>(request.Id), request.Description);

        return new UpdateDescriptionToDoItemReply();
    }

    public override async Task<SkipToDoItemReply> SkipToDoItem(SkipToDoItemRequest request, ServerCallContext context)
    {
        await toDoService.SkipToDoItemAsync(mapper.Map<Guid>(request.Id));

        return new SkipToDoItemReply();
    }

    public override async Task<FailToDoItemReply> FailToDoItem(FailToDoItemRequest request, ServerCallContext context)
    {
        await toDoService.FailToDoItemAsync(mapper.Map<Guid>(request.Id));

        return new FailToDoItemReply();
    }

    public override async Task<SearchReply> Search(SearchRequest request, ServerCallContext context)
    {
        var items = await toDoService.SearchAsync(request.SearchText);
        var reply = new SearchReply();
        reply.Items.AddRange(items.Select(x => mapper.Map<ToDoSubItemGrpc>(x)));

        return reply;
    }

    public override async Task<UpdateToDoItemTypeReply> UpdateToDoItemType(
        UpdateToDoItemTypeRequest request,
        ServerCallContext context
    )
    {
        await toDoService.UpdateToDoItemTypeAsync(mapper.Map<Guid>(request.Id), (ToDoItemType)request.Type);

        return new UpdateToDoItemTypeReply();
    }

    public override async Task<AddCurrentToDoItemReply> AddCurrentToDoItem(
        AddCurrentToDoItemRequest request,
        ServerCallContext context
    )
    {
        await toDoService.AddCurrentToDoItemAsync(mapper.Map<Guid>(request.Id));

        return new AddCurrentToDoItemReply();
    }

    public override async Task<RemoveCurrentToDoItemReply> RemoveCurrentToDoItem(
        RemoveCurrentToDoItemRequest request,
        ServerCallContext context
    )
    {
        await toDoService.RemoveCurrentToDoItemAsync(mapper.Map<Guid>(request.Id));

        return new RemoveCurrentToDoItemReply();
    }

    public override async Task<GetCurrentToDoItemsReply> GetCurrentToDoItems(
        GetCurrentToDoItemsRequest request,
        ServerCallContext context
    )
    {
        var reply = new GetCurrentToDoItemsReply();
        var items = await toDoService.GetCurrentToDoItemsAsync();
        reply.Items.AddRange(mapper.Map<IEnumerable<ToDoSubItemGrpc>>(items));

        return reply;
    }
}