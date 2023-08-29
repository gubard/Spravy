using AutoMapper;
using Google.Protobuf;
using Grpc.Core;
using Spravy.Authentication.Domain.Models;
using Spravy.Client.Exceptions;
using Spravy.Client.Extensions;
using Spravy.Client.Services;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.ToDo.Domain.Client.Models;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.ToDo.Domain.Models;
using Spravy.ToDo.Protos;
using static Spravy.ToDo.Protos.ToDoService;

namespace Spravy.ToDo.Domain.Client.Services;

public class GrpcToDoService : GrpcServiceBase, IToDoService
{
    private readonly ToDoServiceClient client;
    private readonly IMapper mapper;
    private readonly IKeeper<TokenResult> tokenKeeper;

    public GrpcToDoService(GrpcToDoServiceOptions options, IMapper mapper, IKeeper<TokenResult> tokenKeeper)
        : base(options.Host.ToUri(), options.ChannelType, options.ChannelCredentialType.GetChannelCredentials())
    {
        this.mapper = mapper;
        this.tokenKeeper = tokenKeeper;
        client = new ToDoServiceClient(grpcChannel);
    }

    public async Task<IEnumerable<IToDoSubItem>> GetRootToDoSubItemsAsync()
    {
        try
        {
            var items = await client.GetRootToDoSubItemsAsync(new GetRootToDoSubItemsRequest(), CreateMetadata());

            return mapper.Map<IEnumerable<IToDoSubItem>>(items.Items);
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task<IToDoItem> GetToDoItemAsync(Guid id)
    {
        try
        {
            var byteStringId = mapper.Map<ByteString>(id);

            var item = await client.GetToDoItemAsync(
                new GetToDoItemRequest
                {
                    Id = byteStringId
                },
                CreateMetadata()
            );

            var result = mapper.Map<IToDoItem>(item.Item);

            return result;
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task<Guid> AddRootToDoItemAsync(AddRootToDoItemOptions options)
    {
        try
        {
            var id = await client.AddRootToDoItemAsync(mapper.Map<AddRootToDoItemRequest>(options), CreateMetadata());

            return mapper.Map<Guid>(id.Id);
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task<Guid> AddToDoItemAsync(AddToDoItemOptions options)
    {
        try
        {
            var id = await client.AddToDoItemAsync(mapper.Map<AddToDoItemRequest>(options), CreateMetadata());

            return mapper.Map<Guid>(id.Id);
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task DeleteToDoItemAsync(Guid id)
    {
        try
        {
            await client.DeleteToDoItemAsync(
                new DeleteToDoItemRequest
                {
                    Id = mapper.Map<ByteString>(id)
                },
                CreateMetadata()
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task UpdateToDoItemTypeOfPeriodicityAsync(Guid id, TypeOfPeriodicity type)
    {
        try
        {
            await client.UpdateToDoItemTypeOfPeriodicityAsync(
                new UpdateToDoItemTypeOfPeriodicityRequest
                {
                    Id = mapper.Map<ByteString>(id),
                    Type = (TypeOfPeriodicityGrpc)type,
                },
                CreateMetadata()
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task UpdateToDoItemDueDateAsync(Guid id, DateTimeOffset dueDate)
    {
        try
        {
            await client.UpdateToDoItemDueDateAsync(
                new UpdateToDoItemDueDateRequest
                {
                    Id = mapper.Map<ByteString>(id),
                    DueDate = mapper.Map<DateTimeOffsetGrpc>(dueDate),
                },
                CreateMetadata()
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task UpdateToDoItemCompleteStatusAsync(Guid id, bool isCompleted)
    {
        try
        {
            await client.UpdateToDoItemCompleteStatusAsync(
                new UpdateToDoItemCompleteStatusRequest
                {
                    Id = mapper.Map<ByteString>(id),
                    IsCompleted = isCompleted
                },
                CreateMetadata()
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task UpdateToDoItemNameAsync(Guid id, string name)
    {
        try
        {
            await client.UpdateToDoItemNameAsync(
                new UpdateToDoItemNameRequest
                {
                    Id = mapper.Map<ByteString>(id),
                    Name = name,
                },
                CreateMetadata()
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task UpdateToDoItemOrderIndexAsync(UpdateOrderIndexToDoItemOptions options)
    {
        try
        {
            await client.UpdateToDoItemOrderIndexAsync(
                mapper.Map<UpdateToDoItemOrderIndexRequest>(options),
                CreateMetadata()
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task UpdateToDoItemDescriptionAsync(Guid id, string description)
    {
        try
        {
            await client.UpdateToDoItemDescriptionAsync(
                new UpdateToDoItemDescriptionRequest
                {
                    Description = description,
                    Id = mapper.Map<ByteString>(id),
                },
                CreateMetadata()
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task SkipToDoItemAsync(Guid id)
    {
        try
        {
            await client.SkipToDoItemAsync(
                new SkipToDoItemRequest
                {
                    Id = mapper.Map<ByteString>(id),
                },
                CreateMetadata()
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task FailToDoItemAsync(Guid id)
    {
        try
        {
            await client.FailToDoItemAsync(
                new FailToDoItemRequest
                {
                    Id = mapper.Map<ByteString>(id),
                },
                CreateMetadata()
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task<IEnumerable<IToDoSubItem>> SearchToDoSubItemsAsync(string searchText)
    {
        try
        {
            var reply = await client.SearchToDoSubItemsAsync(
                new SearchToDoSubItemsRequest
                {
                    SearchText = searchText
                },
                CreateMetadata()
            );

            return mapper.Map<IEnumerable<IToDoSubItem>>(reply.Items);
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task UpdateToDoItemTypeAsync(Guid id, ToDoItemType type)
    {
        try
        {
            await client.UpdateToDoItemTypeAsync(
                new UpdateToDoItemTypeRequest
                {
                    Id = mapper.Map<ByteString>(id),
                    Type = (ToDoItemTypeGrpc)type,
                },
                CreateMetadata()
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task AddCurrentToDoItemAsync(Guid id)
    {
        try
        {
            await client.AddCurrentToDoItemAsync(
                new AddCurrentToDoItemRequest
                {
                    Id = mapper.Map<ByteString>(id),
                },
                CreateMetadata()
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task RemoveCurrentToDoItemAsync(Guid id)
    {
        try
        {
            await client.RemoveCurrentToDoItemAsync(
                new RemoveCurrentToDoItemRequest
                {
                    Id = mapper.Map<ByteString>(id),
                },
                CreateMetadata()
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task<IEnumerable<IToDoSubItem>> GetCurrentToDoItemsAsync()
    {
        try
        {
            var reply = await client.GetCurrentToDoItemsAsync(new GetCurrentToDoItemsRequest(), CreateMetadata());

            return mapper.Map<IEnumerable<IToDoSubItem>>(reply.Items);
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task UpdateToDoItemAnnuallyPeriodicityAsync(Guid id, AnnuallyPeriodicity periodicity)
    {
        try
        {
            await client.UpdateToDoItemAnnuallyPeriodicityAsync(
                new UpdateToDoItemAnnuallyPeriodicityRequest
                {
                    Periodicity = mapper.Map<AnnuallyPeriodicityGrpc>(periodicity),
                    Id = mapper.Map<ByteString>(id),
                },
                CreateMetadata()
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task UpdateToDoItemMonthlyPeriodicityAsync(Guid id, MonthlyPeriodicity periodicity)
    {
        try
        {
            await client.UpdateToDoItemMonthlyPeriodicityAsync(
                new UpdateToDoItemMonthlyPeriodicityRequest
                {
                    Periodicity = mapper.Map<MonthlyPeriodicityGrpc>(periodicity),
                    Id = mapper.Map<ByteString>(id),
                },
                CreateMetadata()
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task UpdateToDoItemWeeklyPeriodicityAsync(Guid id, WeeklyPeriodicity periodicity)
    {
        try
        {
            await client.UpdateToDoItemWeeklyPeriodicityAsync(
                new UpdateToDoItemWeeklyPeriodicityRequest
                {
                    Periodicity = mapper.Map<WeeklyPeriodicityGrpc>(periodicity),
                    Id = mapper.Map<ByteString>(id),
                },
                CreateMetadata()
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task<IEnumerable<IToDoSubItem>> GetLeafToDoSubItemsAsync(Guid id)
    {
        try
        {
            var reply = await client.GetLeafToDoSubItemsAsync(
                new GetLeafToDoSubItemsRequest
                {
                    Id = mapper.Map<ByteString>(id),
                },
                CreateMetadata()
            );

            return mapper.Map<IEnumerable<IToDoSubItem>>(reply.Items);
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task<IEnumerable<ToDoSelectorItem>> GetToDoSelectorItemsAsync()
    {
        try
        {
            var reply = await client.GetToDoSelectorItemsAsync(
                new GetToDoSelectorItemsRequest(),
                CreateMetadata()
            );

            return mapper.Map<IEnumerable<ToDoSelectorItem>>(reply.Items);
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task UpdateToDoItemParentAsync(Guid id, Guid parentId)
    {
        try
        {
            await client.UpdateToDoItemParentAsync(
                new UpdateToDoItemParentRequest
                {
                    Id = mapper.Map<ByteString>(id),
                    ParentId = mapper.Map<ByteString>(parentId),
                },
                CreateMetadata()
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task ToDoItemToRootAsync(Guid id)
    {
        try
        {
            await client.ToDoItemToRootAsync(
                new ToDoItemToRootRequest
                {
                    Id = mapper.Map<ByteString>(id),
                },
                CreateMetadata()
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task<string> ToDoItemToStringAsync(Guid id)
    {
        try
        {
            var reply = await client.ToDoItemToStringAsync(
                new ToDoItemToStringRequest
                {
                    Id = mapper.Map<ByteString>(id),
                },
                CreateMetadata()
            );

            return reply.Value;
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task InitAsync()
    {
        try
        {
            await client.InitAsync(new InitRequest(), CreateMetadata());
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task UpdateToDoItemDaysOffsetAsync(Guid id, ushort days)
    {
        try
        {
            await client.UpdateToDoItemDaysOffsetAsync(
                new UpdateToDoItemDaysOffsetRequest
                {
                    Id = mapper.Map<ByteString>(id),
                    Days = days
                },
                CreateMetadata()
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task UpdateToDoItemMonthsOffsetAsync(Guid id, ushort months)
    {
        try
        {
            await client.UpdateToDoItemMonthsOffsetAsync(
                new UpdateToDoItemMonthsOffsetRequest
                {
                    Id = mapper.Map<ByteString>(id),
                    Months = months
                },
                CreateMetadata()
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task UpdateToDoItemWeeksOffsetAsync(Guid id, ushort weeks)
    {
        try
        {
            await client.UpdateToDoItemWeeksOffsetAsync(
                new UpdateToDoItemWeeksOffsetRequest
                {
                    Id = mapper.Map<ByteString>(id),
                    Weeks = weeks
                },
                CreateMetadata()
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task UpdateToDoItemYearsOffsetAsync(Guid id, ushort years)
    {
        try
        {
            await client.UpdateToDoItemYearsOffsetAsync(
                new UpdateToDoItemYearsOffsetRequest
                {
                    Id = mapper.Map<ByteString>(id),
                    Years = years
                },
                CreateMetadata()
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task UpdateToDoItemChildrenTypeAsync(Guid id, ToDoItemChildrenType type)
    {
        try
        {
            await client.UpdateToDoItemChildrenTypeAsync(
                new UpdateToDoItemChildrenTypeRequest
                {
                    Id = mapper.Map<ByteString>(id),
                    Type = (ToDoItemChildrenTypeGrpc)type
                },
                CreateMetadata()
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    private Metadata CreateMetadata()
    {
        var metadata = new Metadata
        {
            {
                "Authorization", $"Bearer {tokenKeeper.Get().Token}"
            }
        };

        return metadata;
    }
}