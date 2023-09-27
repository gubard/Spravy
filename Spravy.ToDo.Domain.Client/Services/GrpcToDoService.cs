using AutoMapper;
using Google.Protobuf;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Authentication.Domain.Models;
using Spravy.Client.Exceptions;
using Spravy.Client.Extensions;
using Spravy.Client.Interfaces;
using Spravy.Client.Services;
using Spravy.Domain.Extensions;
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
    private readonly IMetadataFactory metadataFactory;

    public GrpcToDoService(
        GrpcToDoServiceOptions options,
        IMapper mapper,
        ITokenService tokenService,
        IMetadataFactory metadataFactory
    )
        : base(
            options.Host.ThrowIfNull().ToUri(),
            options.ChannelType,
            options.ChannelCredentialType.GetChannelCredentials()
        )
    {
        this.mapper = mapper;
        this.metadataFactory = metadataFactory;
        client = new ToDoServiceClient(GrpcChannel);

        if (!options.Token.IsNullOrWhiteSpace())
        {
            tokenService.Login(new TokenResult(options.Token, options.Token));
        }
    }

    public async Task<IEnumerable<IToDoSubItem>> GetRootToDoSubItemsAsync()
    {
        try
        {
            var metadata = await metadataFactory.CreateAsync();
            var items = await client.GetRootToDoSubItemsAsync(new GetRootToDoSubItemsRequest(), metadata);

            return mapper.Map<IEnumerable<IToDoSubItem>>(items.Items);
        }
        catch (Exception e)
        {
            throw new GrpcException(GrpcChannel.Target, e);
        }
    }

    public async Task<IToDoItem> GetToDoItemAsync(Guid id)
    {
        try
        {
            var byteStringId = mapper.Map<ByteString>(id);

            var request = new GetToDoItemRequest
            {
                Id = byteStringId
            };

            var item = await client.GetToDoItemAsync(request, await metadataFactory.CreateAsync());
            var result = mapper.Map<IToDoItem>(item.Item);

            return result;
        }
        catch (Exception e)
        {
            throw new GrpcException(GrpcChannel.Target, e);
        }
    }

    public async Task<Guid> AddRootToDoItemAsync(AddRootToDoItemOptions options)
    {
        try
        {
            var request = mapper.Map<AddRootToDoItemRequest>(options);
            var id = await client.AddRootToDoItemAsync(request, await metadataFactory.CreateAsync());

            return mapper.Map<Guid>(id.Id);
        }
        catch (Exception e)
        {
            throw new GrpcException(GrpcChannel.Target, e);
        }
    }

    public async Task<Guid> AddToDoItemAsync(AddToDoItemOptions options)
    {
        try
        {
            var request = mapper.Map<AddToDoItemRequest>(options);
            var id = await client.AddToDoItemAsync(request, await metadataFactory.CreateAsync());

            return mapper.Map<Guid>(id.Id);
        }
        catch (Exception e)
        {
            throw new GrpcException(GrpcChannel.Target, e);
        }
    }

    public async Task DeleteToDoItemAsync(Guid id)
    {
        try
        {
            var request = new DeleteToDoItemRequest
            {
                Id = mapper.Map<ByteString>(id)
            };

            await client.DeleteToDoItemAsync(request, await metadataFactory.CreateAsync());
        }
        catch (Exception e)
        {
            throw new GrpcException(GrpcChannel.Target, e);
        }
    }

    public async Task UpdateToDoItemTypeOfPeriodicityAsync(Guid id, TypeOfPeriodicity type)
    {
        try
        {
            var request = new UpdateToDoItemTypeOfPeriodicityRequest
            {
                Id = mapper.Map<ByteString>(id),
                Type = (TypeOfPeriodicityGrpc)type,
            };

            await client.UpdateToDoItemTypeOfPeriodicityAsync(request, await metadataFactory.CreateAsync());
        }
        catch (Exception e)
        {
            throw new GrpcException(GrpcChannel.Target, e);
        }
    }

    public async Task UpdateToDoItemDueDateAsync(Guid id, DateTimeOffset dueDate)
    {
        try
        {
            var request = new UpdateToDoItemDueDateRequest
            {
                Id = mapper.Map<ByteString>(id),
                DueDate = mapper.Map<DateTimeOffsetGrpc>(dueDate),
            };

            await client.UpdateToDoItemDueDateAsync(request, await metadataFactory.CreateAsync());
        }
        catch (Exception e)
        {
            throw new GrpcException(GrpcChannel.Target, e);
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
                await metadataFactory.CreateAsync()
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(GrpcChannel.Target, e);
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
                await metadataFactory.CreateAsync()
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(GrpcChannel.Target, e);
        }
    }

    public async Task UpdateToDoItemOrderIndexAsync(UpdateOrderIndexToDoItemOptions options)
    {
        try
        {
            await client.UpdateToDoItemOrderIndexAsync(
                mapper.Map<UpdateToDoItemOrderIndexRequest>(options),
                await metadataFactory.CreateAsync()
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(GrpcChannel.Target, e);
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
                await metadataFactory.CreateAsync()
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(GrpcChannel.Target, e);
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
                await metadataFactory.CreateAsync()
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(GrpcChannel.Target, e);
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
                await metadataFactory.CreateAsync()
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(GrpcChannel.Target, e);
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
                await metadataFactory.CreateAsync()
            );

            return mapper.Map<IEnumerable<IToDoSubItem>>(reply.Items);
        }
        catch (Exception e)
        {
            throw new GrpcException(GrpcChannel.Target, e);
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
                await metadataFactory.CreateAsync()
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(GrpcChannel.Target, e);
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
                await metadataFactory.CreateAsync()
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(GrpcChannel.Target, e);
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
                await metadataFactory.CreateAsync()
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(GrpcChannel.Target, e);
        }
    }

    public async Task<IEnumerable<IToDoSubItem>> GetCurrentToDoItemsAsync()
    {
        try
        {
            var reply = await client.GetCurrentToDoItemsAsync(
                new GetCurrentToDoItemsRequest(),
                await metadataFactory.CreateAsync()
            );

            return mapper.Map<IEnumerable<IToDoSubItem>>(reply.Items);
        }
        catch (Exception e)
        {
            throw new GrpcException(GrpcChannel.Target, e);
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
                await metadataFactory.CreateAsync()
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(GrpcChannel.Target, e);
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
                await metadataFactory.CreateAsync()
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(GrpcChannel.Target, e);
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
                await metadataFactory.CreateAsync()
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(GrpcChannel.Target, e);
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
                await metadataFactory.CreateAsync()
            );

            return mapper.Map<IEnumerable<IToDoSubItem>>(reply.Items);
        }
        catch (Exception e)
        {
            throw new GrpcException(GrpcChannel.Target, e);
        }
    }

    public async Task<IEnumerable<ToDoSelectorItem>> GetToDoSelectorItemsAsync(Guid[] ignoreIds)
    {
        try
        {
            var request = new GetToDoSelectorItemsRequest();
            request.IgnoreIds.AddRange(mapper.Map<IEnumerable<ByteString>>(ignoreIds));
            var reply = await client.GetToDoSelectorItemsAsync(request, await metadataFactory.CreateAsync());

            return mapper.Map<IEnumerable<ToDoSelectorItem>>(reply.Items);
        }
        catch (Exception e)
        {
            throw new GrpcException(GrpcChannel.Target, e);
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
                await metadataFactory.CreateAsync()
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(GrpcChannel.Target, e);
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
                await metadataFactory.CreateAsync()
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(GrpcChannel.Target, e);
        }
    }

    public async Task<string> ToDoItemToStringAsync(ToDoItemToStringOptions options)
    {
        try
        {
            var request = mapper.Map<ToDoItemToStringRequest>(options);
            var reply = await client.ToDoItemToStringAsync(request, await metadataFactory.CreateAsync());

            return reply.Value;
        }
        catch (Exception e)
        {
            throw new GrpcException(GrpcChannel.Target, e);
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
                await metadataFactory.CreateAsync()
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(GrpcChannel.Target, e);
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
                await metadataFactory.CreateAsync()
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(GrpcChannel.Target, e);
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
                await metadataFactory.CreateAsync()
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(GrpcChannel.Target, e);
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
                await metadataFactory.CreateAsync()
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(GrpcChannel.Target, e);
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
                await metadataFactory.CreateAsync()
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(GrpcChannel.Target, e);
        }
    }
}