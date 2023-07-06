using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ExtensionFramework.Core.Common.Extensions;
using Google.Protobuf.WellKnownTypes;
using Google.Protobuf;
using Spravy.Core.Enums;
using Spravy.Core.Interfaces;
using Spravy.Core.Models;
using Spravy.Models;
using Spravy.Protos;

namespace Spravy.Services;

public class GrpcToDoService : GrpcServiceBase, IToDoService
{
    private readonly ToDoService.ToDoServiceClient client;
    private readonly IMapper mapper;

    public GrpcToDoService(GrpcToDoServiceOptions options, IMapper mapper) : base(options.Host.ToUri())
    {
        this.mapper = mapper;
        client = new ToDoService.ToDoServiceClient(grpcChannel);
    }

    public async Task<IEnumerable<ToDoSubItem>> GetRootToDoItemsAsync()
    {
        var items = await client.GetRootToDoItemsAsync(new GetRootToDoItemsRequest());

        return items.Items.Select(x => mapper.Map<ToDoSubItem>(x)).ToArray();
    }

    public async Task<ToDoItem> GetToDoItemAsync(Guid id)
    {
        var item = await client.GetToDoItemAsync(
            new GetToDoItemRequest
            {
                Id = mapper.Map<ByteString>(id),
            }
        );

        return mapper.Map<ToDoItem>(item.Item);
    }

    public async Task<Guid> AddRootToDoItemAsync(AddRootToDoItemOptions options)
    {
        var id = await client.AddRootToDoItemAsync(mapper.Map<AddRootToDoItemRequest>(options));

        return mapper.Map<Guid>(id.Id);
    }

    public async Task<Guid> AddToDoItemAsync(AddToDoItemOptions options)
    {
        var id = await client.AddToDoItemAsync(mapper.Map<AddToDoItemRequest>(options));

        return mapper.Map<Guid>(id.Id);
    }

    public async Task DeleteToDoItemAsync(Guid id)
    {
        await client.DeleteToDoItemAsync(
            new DeleteToDoItemRequest
            {
                Id = mapper.Map<ByteString>(id)
            }
        );
    }

    public async Task UpdateTypeOfPeriodicityAsync(Guid id, TypeOfPeriodicity type)
    {
        await client.UpdateTypeOfPeriodicityAsync(
            new UpdateTypeOfPeriodicityRequest
            {
                Id = mapper.Map<ByteString>(id),
                Type = (TypeOfPeriodicityGrpc)type,
            }
        );
    }

    public async Task UpdateDueDateAsync(Guid id, DateTimeOffset? dueDate)
    {
        await client.UpdateDueDateAsync(
            new UpdateDueDateRequest
            {
                Id = mapper.Map<ByteString>(id),
                DueDate = mapper.Map<DateTimeOffsetGrpc>(dueDate),
            }
        );
    }

    public async Task UpdateCompleteStatusAsync(Guid id, bool isComplete)
    {
        await client.UpdateCompleteStatusAsync(
            new UpdateCompleteStatusRequest
            {
                Id = mapper.Map<ByteString>(id),
                IsComplete = isComplete
            }
        );
    }

    public async Task UpdateNameToDoItemAsync(Guid id, string name)
    {
        await client.UpdateNameToDoItemAsync(
            new UpdateNameToDoItemRequest
            {
                Id = mapper.Map<ByteString>(id),
                Name = name,
            }
        );
    }

    public async Task UpdateOrderIndexToDoItemAsync(UpdateOrderIndexToDoItemOptions options)
    {
        await client.UpdateOrderIndexToDoItemAsync(mapper.Map<UpdateOrderIndexToDoItemRequest>(options));
    }
}