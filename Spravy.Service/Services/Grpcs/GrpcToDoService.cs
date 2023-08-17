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

    public override async Task<GetRootToDoSubItemsReply> GetRootToDoSubItems(
        GetRootToDoSubItemsRequest request,
        ServerCallContext context
    )
    {
        var reply = new GetRootToDoSubItemsReply();
        var items = await toDoService.GetRootToDoSubItemsAsync();
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

    public override async Task<UpdateToDoItemTypeOfPeriodicityReply> UpdateToDoItemTypeOfPeriodicity(
        UpdateToDoItemTypeOfPeriodicityRequest request,
        ServerCallContext context
    )
    {
        await toDoService.UpdateToDoItemTypeOfPeriodicityAsync(
            mapper.Map<Guid>(request.Id),
            (TypeOfPeriodicity)request.Type
        );

        return new UpdateToDoItemTypeOfPeriodicityReply();
    }

    public override async Task<UpdateToDoItemDueDateReply> UpdateToDoItemDueDate(
        UpdateToDoItemDueDateRequest request,
        ServerCallContext context
    )
    {
        var dueDate = mapper.Map<DateTimeOffset>(request.DueDate);
        await toDoService.UpdateToDoItemDueDateAsync(mapper.Map<Guid>(request.Id), dueDate);

        return new UpdateToDoItemDueDateReply();
    }

    public override async Task<UpdateToDoItemCompleteStatusReply> UpdateToDoItemCompleteStatus(
        UpdateToDoItemCompleteStatusRequest request,
        ServerCallContext context
    )
    {
        await toDoService.UpdateToDoItemCompleteStatusAsync(mapper.Map<Guid>(request.Id), request.IsCompleted);

        return new UpdateToDoItemCompleteStatusReply();
    }

    public override async Task<UpdateToDoItemNameReply> UpdateToDoItemName(
        UpdateToDoItemNameRequest request,
        ServerCallContext context
    )
    {
        await toDoService.UpdateToDoItemNameAsync(mapper.Map<Guid>(request.Id), request.Name);

        return new UpdateToDoItemNameReply();
    }

    public override async Task<UpdateToDoItemOrderIndexReply> UpdateToDoItemOrderIndex(
        UpdateToDoItemOrderIndexRequest request,
        ServerCallContext context
    )
    {
        var options = mapper.Map<UpdateOrderIndexToDoItemOptions>(request);
        await toDoService.UpdateToDoItemOrderIndexAsync(options);

        return new UpdateToDoItemOrderIndexReply();
    }

    public override async Task<UpdateToDoItemDescriptionReply> UpdateToDoItemDescription(
        UpdateToDoItemDescriptionRequest request,
        ServerCallContext context
    )
    {
        await toDoService.UpdateToDoItemDescriptionAsync(mapper.Map<Guid>(request.Id), request.Description);

        return new UpdateToDoItemDescriptionReply();
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

    public override async Task<SearchToDoSubItemsReply> SearchToDoSubItems(
        SearchToDoSubItemsRequest request,
        ServerCallContext context
    )
    {
        var items = await toDoService.SearchToDoSubItemsAsync(request.SearchText);
        var reply = new SearchToDoSubItemsReply();
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

    public override async Task<UpdateToDoItemAnnuallyPeriodicityReply> UpdateToDoItemAnnuallyPeriodicity(
        UpdateToDoItemAnnuallyPeriodicityRequest request,
        ServerCallContext context
    )
    {
        await toDoService.UpdateToDoItemAnnuallyPeriodicityAsync(
            mapper.Map<Guid>(request.Id),
            mapper.Map<AnnuallyPeriodicity>(request.Periodicity)
        );

        return new UpdateToDoItemAnnuallyPeriodicityReply();
    }

    public override async Task<UpdateToDoItemMonthlyPeriodicityReply> UpdateToDoItemMonthlyPeriodicity(
        UpdateToDoItemMonthlyPeriodicityRequest request,
        ServerCallContext context
    )
    {
        await toDoService.UpdateToDoItemMonthlyPeriodicityAsync(
            mapper.Map<Guid>(request.Id),
            mapper.Map<MonthlyPeriodicity>(request.Periodicity)
        );

        return new UpdateToDoItemMonthlyPeriodicityReply();
    }

    public override async Task<UpdateToDoItemWeeklyPeriodicityReply> UpdateToDoItemWeeklyPeriodicity(
        UpdateToDoItemWeeklyPeriodicityRequest request,
        ServerCallContext context
    )
    {
        await toDoService.UpdateToDoItemWeeklyPeriodicityAsync(
            mapper.Map<Guid>(request.Id),
            mapper.Map<WeeklyPeriodicity>(request.Periodicity)
        );

        return new UpdateToDoItemWeeklyPeriodicityReply();
    }

    public override async Task<GetLeafToDoSubItemsReply> GetLeafToDoSubItems(
        GetLeafToDoSubItemsRequest request,
        ServerCallContext context
    )
    {
        var items = await toDoService.GetLeafToDoSubItemsAsync(mapper.Map<Guid>(request.Id));
        var reply = new GetLeafToDoSubItemsReply();
        reply.Items.AddRange(mapper.Map<IEnumerable<ToDoSubItemGrpc>>(items));

        return reply;
    }

    public override async Task<GetToDoSelectorItemsReply> GetToDoSelectorItems(
        GetToDoSelectorItemsRequest request,
        ServerCallContext context
    )
    {
        var items = await toDoService.GetToDoSelectorItemsAsync();
        var reply = new GetToDoSelectorItemsReply();
        reply.Items.AddRange(mapper.Map<IEnumerable<ToDoSelectorItemGrpc>>(items));

        return reply;
    }

    public override async Task<UpdateToDoItemParentReply> UpdateToDoItemParent(
        UpdateToDoItemParentRequest request,
        ServerCallContext context
    )
    {
        await toDoService.UpdateToDoItemParentAsync(mapper.Map<Guid>(request.Id), mapper.Map<Guid>(request.ParentId));

        return new UpdateToDoItemParentReply();
    }

    public override async Task<ToDoItemToRootReply> ToDoItemToRoot(
        ToDoItemToRootRequest request,
        ServerCallContext context
    )
    {
        await toDoService.ToDoItemToRootAsync(mapper.Map<Guid>(request.Id));

        return new ToDoItemToRootReply();
    }

    public override async Task<ToDoItemToStringReply> ToDoItemToString(
        ToDoItemToStringRequest request,
        ServerCallContext context
    )
    {
        var value = await toDoService.ToDoItemToStringAsync(mapper.Map<Guid>(request.Id));

        return new ToDoItemToStringReply
        {
            Value = value,
        };
    }
}