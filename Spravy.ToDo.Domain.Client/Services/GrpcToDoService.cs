using System.Runtime.CompilerServices;
using Grpc.Core;
using Spravy.Client.Extensions;
using Spravy.Client.Interfaces;
using Spravy.Client.Services;
using Spravy.Core.Mappers;
using Spravy.Domain.Enums;
using Spravy.Domain.Extensions;
using Spravy.Domain.Helpers;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.ToDo.Domain.Mapper.Mappers;
using Spravy.ToDo.Domain.Models;
using Spravy.ToDo.Protos;

namespace Spravy.ToDo.Domain.Client.Services;

public class GrpcToDoService : GrpcServiceBase<ToDoService.ToDoServiceClient>,
    IToDoService,
    IGrpcServiceCreatorAuth<GrpcToDoService, ToDoService.ToDoServiceClient>
{
    public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(30);

    private readonly IMetadataFactory metadataFactory;

    public GrpcToDoService(
        IFactory<Uri, ToDoService.ToDoServiceClient> grpcClientFactory,
        Uri host,
        IMetadataFactory metadataFactory,
        ISerializer serializer
    ) : base(grpcClientFactory, host, serializer)
    {
        this.metadataFactory = metadataFactory;
    }

    public static GrpcToDoService CreateGrpcService(
        IFactory<Uri, ToDoService.ToDoServiceClient> grpcClientFactory,
        Uri host,
        IMetadataFactory metadataFactory,
        ISerializer serializer
    )
    {
        return new(grpcClientFactory, host, metadataFactory, serializer);
    }

    public ConfiguredValueTaskAwaitable<Result> CloneToDoItemAsync(
        Guid cloneId,
        OptionStruct<Guid> parentId,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(metadata => client.CloneToDoItemAsync(new()
                {
                    CloneId = cloneId.ToByteString(),
                    ParentId = parentId.ToByteString(),
                }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
               .ToValueTaskResultOnly()
               .ConfigureAwait(false), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemDescriptionTypeAsync(
        Guid id,
        DescriptionType type,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(metadata => client.UpdateToDoItemDescriptionTypeAsync(new()
                {
                    Id = id.ToByteString(),
                    Type = (DescriptionTypeGrpc)type,
                }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
               .ToValueTaskResultOnly()
               .ConfigureAwait(false), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result<ReferenceToDoItemSettings>> GetReferenceToDoItemSettingsAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(metadata => client.GetReferenceToDoItemSettingsAsync(new()
                {
                    Id = id.ToByteString(),
                }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
               .ToValueTaskResultValueOnly()
               .ConfigureAwait(false), cancellationToken)
           .IfSuccessAsync(reply => reply.ToReferenceToDoItemSettings().ToResult(), cancellationToken),
        cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateReferenceToDoItemAsync(
        Guid id,
        Guid referenceId,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(metadata => client.UpdateReferenceToDoItemAsync(new()
                {
                    Id = id.ToByteString(),
                    ReferenceId = referenceId.ToByteString(),
                }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
               .ToValueTaskResultOnly()
               .ConfigureAwait(false), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> ResetToDoItemAsync(
        ResetToDoItemOptions options,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(cancellationToken)
               .IfSuccessAsync(
                    metadata => client
                       .ResetToDoItemAsync(options.ToResetToDoItemRequest(), metadata, DateTime.UtcNow.Add(Timeout),
                            cancellationToken)
                       .ToValueTaskResultOnly()
                       .ConfigureAwait(false), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> RandomizeChildrenOrderIndexAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(metadata => client.RandomizeChildrenOrderIndexAsync(new()
                {
                    Id = id.ToByteString(),
                }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
               .ToValueTaskResultOnly()
               .ConfigureAwait(false), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<ToDoShortItem>>> GetParentsAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
               .IfSuccessAsync(metadata => client.GetParentsAsync(new()
                    {
                        Id = id.ToByteString(),
                    }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
                   .ToValueTaskResultValueOnly()
                   .ConfigureAwait(false)
                   .IfSuccessAsync(reply => reply.Parents.ToToDoShortItem().ToResult(), cancellationToken),
                cancellationToken),
            cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> SearchToDoItemIdsAsync(
        string searchText,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
               .IfSuccessAsync(metadata => client.SearchToDoItemIdsAsync(new()
                    {
                        SearchText = searchText,
                    }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
                   .ToValueTaskResultValueOnly()
                   .ConfigureAwait(false)
                   .IfSuccessAsync(reply => reply.Ids.ToGuid().ToResult(), cancellationToken), cancellationToken),
            cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetLeafToDoItemIdsAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
               .IfSuccessAsync(metadata => client.GetLeafToDoItemIdsAsync(new()
                    {
                        Id = id.ToByteString(),
                    }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
                   .ToValueTaskResultValueOnly()
                   .ConfigureAwait(false)
                   .IfSuccessAsync(reply => reply.Ids.ToGuid().ToResult(), cancellationToken), cancellationToken),
            cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result<ToDoItem>> GetToDoItemAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
               .IfSuccessAsync(metadata => client.GetToDoItemAsync(new()
                    {
                        Id = id.ToByteString(),
                    }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
                   .ToValueTaskResultValueOnly()
                   .ConfigureAwait(false)
                   .IfSuccessAsync(reply => reply.ToToDoItem().ToResult(), cancellationToken), cancellationToken),
            cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetChildrenToDoItemIdsAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
               .IfSuccessAsync(metadata => client.GetChildrenToDoItemIdsAsync(new()
                    {
                        Id = id.ToByteString(),
                    }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
                   .ToValueTaskResultValueOnly()
                   .ConfigureAwait(false)
                   .IfSuccessAsync(reply => reply.Ids.ToGuid().ToResult(), cancellationToken), cancellationToken),
            cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<ToDoShortItem>>> GetChildrenToDoItemShortsAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
               .IfSuccessAsync(metadata => client.GetChildrenToDoItemShortsAsync(new()
                    {
                        Id = id.ToByteString(),
                    }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
                   .ToValueTaskResultValueOnly()
                   .ConfigureAwait(false)
                   .IfSuccessAsync(reply => reply.Items.ToToDoShortItem().ToResult(), cancellationToken),
                cancellationToken),
            cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetRootToDoItemIdsAsync(
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(cancellationToken)
               .IfSuccessAsync(
                    metadata => client
                       .GetRootToDoItemIdsAsync(new(), metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
                       .ToValueTaskResultValueOnly()
                       .ConfigureAwait(false)
                       .IfSuccessAsync(reply => reply.Ids.ToGuid().ToResult(), cancellationToken), cancellationToken),
            cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetFavoriteToDoItemIdsAsync(
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(cancellationToken)
               .IfSuccessAsync(
                    metadata => client
                       .GetFavoriteToDoItemIdsAsync(new(), metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
                       .ToValueTaskResultValueOnly()
                       .ConfigureAwait(false)
                       .IfSuccessAsync(reply => reply.Ids.ToGuid().ToResult(), cancellationToken), cancellationToken),
            cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result<Guid>> AddRootToDoItemAsync(
        AddRootToDoItemOptions options,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(cancellationToken)
               .IfSuccessAsync(
                    metadata => client
                       .AddRootToDoItemAsync(options.ToAddRootToDoItemRequest(), metadata, DateTime.UtcNow.Add(Timeout),
                            cancellationToken)
                       .ToValueTaskResultValueOnly()
                       .ConfigureAwait(false)
                       .IfSuccessAsync(id => id.Id.ToGuid().ToResult(), cancellationToken), cancellationToken),
            cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result<Guid>> AddToDoItemAsync(
        AddToDoItemOptions options,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(cancellationToken)
               .IfSuccessAsync(
                    metadata => client
                       .AddToDoItemAsync(options.ToAddToDoItemRequest(), metadata, DateTime.UtcNow.Add(Timeout),
                            cancellationToken)
                       .ToValueTaskResultValueOnly()
                       .ConfigureAwait(false)
                       .IfSuccessAsync(id => id.Id.ToGuid().ToResult(), cancellationToken), cancellationToken),
            cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> DeleteToDoItemAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(metadata => client.DeleteToDoItemAsync(new()
                {
                    Id = id.ToByteString(),
                }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
               .ToValueTaskResultOnly()
               .ConfigureAwait(false), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemTypeOfPeriodicityAsync(
        Guid id,
        TypeOfPeriodicity type,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(metadata => client.UpdateToDoItemTypeOfPeriodicityAsync(new()
                {
                    Id = id.ToByteString(),
                    Type = (TypeOfPeriodicityGrpc)type,
                }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
               .ToValueTaskResultOnly()
               .ConfigureAwait(false), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemDueDateAsync(
        Guid id,
        DateOnly dueDate,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(metadata => client.UpdateToDoItemDueDateAsync(new()
                {
                    Id = id.ToByteString(),
                    DueDate = dueDate.ToTimestamp(),
                }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
               .ToValueTaskResultOnly()
               .ConfigureAwait(false), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemCompleteStatusAsync(
        Guid id,
        bool isComplete,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(metadata => client.UpdateToDoItemCompleteStatusAsync(new()
                {
                    Id = id.ToByteString(),
                    IsCompleted = isComplete,
                }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
               .ToValueTaskResultOnly()
               .ConfigureAwait(false), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemNameAsync(
        Guid id,
        string name,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(metadata => client.UpdateToDoItemNameAsync(new()
                {
                    Id = id.ToByteString(),
                    Name = name,
                }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
               .ToValueTaskResultOnly()
               .ConfigureAwait(false), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemOrderIndexAsync(
        UpdateOrderIndexToDoItemOptions options,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(cancellationToken)
               .IfSuccessAsync(
                    metadata => client
                       .UpdateToDoItemOrderIndexAsync(options.ToUpdateToDoItemOrderIndexRequest(), metadata,
                            DateTime.UtcNow.Add(Timeout), cancellationToken)
                       .ToValueTaskResultOnly()
                       .ConfigureAwait(false), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemDescriptionAsync(
        Guid id,
        string description,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(metadata => client.UpdateToDoItemDescriptionAsync(new()
                {
                    Description = description,
                    Id = id.ToByteString(),
                }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
               .ToValueTaskResultOnly()
               .ConfigureAwait(false), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemTypeAsync(
        Guid id,
        ToDoItemType type,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(metadata => client.UpdateToDoItemTypeAsync(new()
                {
                    Id = id.ToByteString(),
                    Type = (ToDoItemTypeGrpc)type,
                }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
               .ToValueTaskResultOnly()
               .ConfigureAwait(false), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> AddFavoriteToDoItemAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(metadata => client.AddFavoriteToDoItemAsync(new()
                {
                    Id = id.ToByteString(),
                }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
               .ToValueTaskResultOnly()
               .ConfigureAwait(false), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> RemoveFavoriteToDoItemAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(metadata => client.RemoveFavoriteToDoItemAsync(new()
                {
                    Id = id.ToByteString(),
                }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
               .ToValueTaskResultOnly()
               .ConfigureAwait(false), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemIsRequiredCompleteInDueDateAsync(
        Guid id,
        bool value,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(metadata => client.UpdateToDoItemIsRequiredCompleteInDueDateAsync(new()
                {
                    Id = id.ToByteString(),
                    IsRequiredCompleteInDueDate = value,
                }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
               .ToValueTaskResultOnly()
               .ConfigureAwait(false), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetTodayToDoItemsAsync(
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(cancellationToken)
               .IfSuccessAsync(
                    metadata => client
                       .GetTodayToDoItemsAsync(new(), metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
                       .ToValueTaskResultValueOnly()
                       .ConfigureAwait(false)
                       .IfSuccessAsync(reply => reply.Ids.ToGuid().ToResult(), cancellationToken), cancellationToken),
            cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemAnnuallyPeriodicityAsync(
        Guid id,
        AnnuallyPeriodicity periodicity,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(metadata => client.UpdateToDoItemAnnuallyPeriodicityAsync(new()
                {
                    Periodicity = periodicity.ToAnnuallyPeriodicityGrpc(),
                    Id = id.ToByteString(),
                }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
               .ToValueTaskResultOnly()
               .ConfigureAwait(false), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemMonthlyPeriodicityAsync(
        Guid id,
        MonthlyPeriodicity periodicity,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(metadata => client.UpdateToDoItemMonthlyPeriodicityAsync(new()
                {
                    Periodicity = periodicity.ToMonthlyPeriodicityGrpc(),
                    Id = id.ToByteString(),
                }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
               .ToValueTaskResultOnly()
               .ConfigureAwait(false), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemWeeklyPeriodicityAsync(
        Guid id,
        WeeklyPeriodicity periodicity,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(metadata => client.UpdateToDoItemWeeklyPeriodicityAsync(new()
                {
                    Periodicity = periodicity.ToWeeklyPeriodicityGrpc(),
                    Id = id.ToByteString(),
                }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
               .ToValueTaskResultOnly()
               .ConfigureAwait(false), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<ToDoSelectorItem>>> GetToDoSelectorItemsAsync(
        ReadOnlyMemory<Guid> ignoreIds,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(metadata =>
            {
                var request = new GetToDoSelectorItemsRequest();
                request.IgnoreIds.AddRange(ignoreIds.ToByteString().ToArray());

                return client
                   .GetToDoSelectorItemsAsync(request, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
                   .ToValueTaskResultValueOnly()
                   .ConfigureAwait(false)
                   .IfSuccessAsync(reply => reply.Items.ToToDoSelectorItem().ToResult(), cancellationToken);
            }, cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemParentAsync(
        Guid id,
        Guid parentId,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(metadata => client.UpdateToDoItemParentAsync(new()
                {
                    Id = id.ToByteString(),
                    ParentId = parentId.ToByteString(),
                }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
               .ToValueTaskResultOnly()
               .ConfigureAwait(false), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> ToDoItemToRootAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(metadata => client.ToDoItemToRootAsync(new()
                {
                    Id = id.ToByteString(),
                }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
               .ToValueTaskResultOnly()
               .ConfigureAwait(false), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result<string>> ToDoItemToStringAsync(
        ToDoItemToStringOptions options,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(cancellationToken)
               .IfSuccessAsync(
                    metadata => client
                       .ToDoItemToStringAsync(options.ToToDoItemToStringRequest(), metadata,
                            DateTime.UtcNow.Add(Timeout), cancellationToken)
                       .ToValueTaskResultValueOnly()
                       .ConfigureAwait(false)
                       .IfSuccessAsync(reply => reply.Value.ToResult().ToValueTaskResult().ConfigureAwait(false),
                            cancellationToken), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemDaysOffsetAsync(
        Guid id,
        ushort days,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(metadata => client.UpdateToDoItemDaysOffsetAsync(new()
                {
                    Id = id.ToByteString(),
                    Days = days,
                }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
               .ToValueTaskResultOnly()
               .ConfigureAwait(false), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemMonthsOffsetAsync(
        Guid id,
        ushort months,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(metadata => client.UpdateToDoItemMonthsOffsetAsync(new()
                {
                    Id = id.ToByteString(),
                    Months = months,
                }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
               .ToValueTaskResultOnly()
               .ConfigureAwait(false), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemWeeksOffsetAsync(
        Guid id,
        ushort weeks,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(metadata => client.UpdateToDoItemWeeksOffsetAsync(new()
                {
                    Id = id.ToByteString(),
                    Weeks = weeks,
                }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
               .ToValueTaskResultOnly()
               .ConfigureAwait(false), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemYearsOffsetAsync(
        Guid id,
        ushort years,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(metadata => client.UpdateToDoItemYearsOffsetAsync(new()
                {
                    Id = id.ToByteString(),
                    Years = years,
                }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
               .ToValueTaskResultOnly()
               .ConfigureAwait(false), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemChildrenTypeAsync(
        Guid id,
        ToDoItemChildrenType type,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(metadata => client.UpdateToDoItemChildrenTypeAsync(new()
                {
                    Id = id.ToByteString(),
                    Type = (ToDoItemChildrenTypeGrpc)type,
                }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
               .ToValueTaskResultOnly()
               .ConfigureAwait(false), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<ToDoShortItem>>> GetSiblingsAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
               .IfSuccessAsync(metadata => client.GetSiblingsAsync(new()
                    {
                        Id = id.ToByteString(),
                    }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
                   .ToValueTaskResultValueOnly()
                   .ConfigureAwait(false)
                   .IfSuccessAsync(items => items.Items.ToToDoShortItem().ToResult(), cancellationToken),
                cancellationToken),
            cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result<OptionStruct<ActiveToDoItem>>> GetCurrentActiveToDoItemAsync(
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(cancellationToken)
               .IfSuccessAsync(
                    metadata => client
                       .GetCurrentActiveToDoItemAsync(DefaultObject<GetCurrentActiveToDoItemRequest>.Default, metadata,
                            DateTime.UtcNow.Add(Timeout), cancellationToken)
                       .ToValueTaskResultValueOnly()
                       .ConfigureAwait(false)
                       .IfSuccessAsync(reply => reply.Item.ToOptionActiveToDoItem().ToResult(), cancellationToken),
                    cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemLinkAsync(
        Guid id,
        Option<Uri> link,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(metadata => client.UpdateToDoItemLinkAsync(new()
                {
                    Id = id.ToByteString(),
                    Link = link.MapToString(),
                }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
               .ToValueTaskResultOnly()
               .ConfigureAwait(false), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result<PlannedToDoItemSettings>> GetPlannedToDoItemSettingsAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
               .IfSuccessAsync(metadata => client.GetPlannedToDoItemSettingsAsync(new()
                    {
                        Id = id.ToByteString(),
                    }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
                   .ToValueTaskResultValueOnly()
                   .ConfigureAwait(false)
                   .IfSuccessAsync(reply => reply.ToPlannedToDoItemSettings().ToResult(), cancellationToken),
                cancellationToken),
            cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result<ValueToDoItemSettings>> GetValueToDoItemSettingsAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
               .IfSuccessAsync(metadata => client.GetValueToDoItemSettingsAsync(new()
                    {
                        Id = id.ToByteString(),
                    }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
                   .ToValueTaskResultValueOnly()
                   .ConfigureAwait(false)
                   .IfSuccessAsync(reply => reply.ToValueToDoItemSettings().ToResult(), cancellationToken),
                cancellationToken),
            cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result<PeriodicityToDoItemSettings>> GetPeriodicityToDoItemSettingsAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(metadata => client.GetPeriodicityToDoItemSettingsAsync(new()
                    {
                        Id = id.ToByteString(),
                    }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
                   .ToValueTaskResultValueOnly()
                   .ConfigureAwait(false)
                   .IfSuccessAsync(reply => reply.ToPeriodicityToDoItemSettings().ToResult(), cancellationToken),
                cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result<WeeklyPeriodicity>> GetWeeklyPeriodicityAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
               .IfSuccessAsync(metadata => client.GetWeeklyPeriodicityAsync(new()
                    {
                        Id = id.ToByteString(),
                    }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
                   .ToValueTaskResultValueOnly()
                   .ConfigureAwait(false)
                   .IfSuccessAsync(reply => reply.ToWeeklyPeriodicity().ToResult(), cancellationToken),
                cancellationToken),
            cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result<MonthlyPeriodicity>> GetMonthlyPeriodicityAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
               .IfSuccessAsync(metadata => client.GetMonthlyPeriodicityAsync(new()
                    {
                        Id = id.ToByteString(),
                    }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
                   .ToValueTaskResultValueOnly()
                   .ConfigureAwait(false)
                   .IfSuccessAsync(reply => reply.ToMonthlyPeriodicity().ToResult(), cancellationToken),
                cancellationToken),
            cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result<AnnuallyPeriodicity>> GetAnnuallyPeriodicityAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
               .IfSuccessAsync(metadata => client.GetAnnuallyPeriodicityAsync(new()
                    {
                        Id = id.ToByteString(),
                    }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
                   .ToValueTaskResultValueOnly()
                   .ConfigureAwait(false)
                   .IfSuccessAsync(reply => reply.ToAnnuallyPeriodicity().ToResult(), cancellationToken),
                cancellationToken),
            cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result<PeriodicityOffsetToDoItemSettings>>
        GetPeriodicityOffsetToDoItemSettingsAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(metadata => client.GetPeriodicityOffsetToDoItemSettingsAsync(new()
                    {
                        Id = id.ToByteString(),
                    }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
                   .ToValueTaskResultValueOnly()
                   .ConfigureAwait(false)
                   .IfSuccessAsync(reply => reply.ToPeriodicityOffsetToDoItemSettings().ToResult(), cancellationToken),
                cancellationToken), cancellationToken);
    }

    public ConfiguredCancelableAsyncEnumerable<Result<ReadOnlyMemory<ToDoItem>>> GetToDoItemsAsync(
        ReadOnlyMemory<Guid> ids,
        uint chunkSize,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync((client, token) => GetToDoItemsCore(client, ids, chunkSize, token).ConfigureAwait(false),
            cancellationToken);
    }

    private async IAsyncEnumerable<Result<ReadOnlyMemory<ToDoItem>>> GetToDoItemsCore(
        ToDoService.ToDoServiceClient client,
        ReadOnlyMemory<Guid> ids,
        uint chunkSize,
        [EnumeratorCancellation] CancellationToken cancellationToken
    )
    {
        if (ids.IsEmpty)
        {
            yield return ReadOnlyMemory<ToDoItem>.Empty.ToResult();

            yield break;
        }

        var request = new GetToDoItemsRequest
        {
            ChunkSize = chunkSize,
        };

        var idsByteString = ids.ToByteString();
        request.Ids.AddRange(idsByteString.ToArray());
        var metadata = await metadataFactory.CreateAsync(cancellationToken);

        if (metadata.IsHasError)
        {
            yield return new(metadata.Errors);

            yield break;
        }

        using var response =
            client.GetToDoItems(request, metadata.Value, DateTime.UtcNow.Add(Timeout), cancellationToken);

        while (await MoveNextAsync(response, cancellationToken))
        {
            var reply = response.ResponseStream.Current;
            var item = reply.Items.ToToDoItem().ToResult();

            yield return item;
        }
    }

    private async ValueTask<bool> MoveNextAsync<T>(
        AsyncServerStreamingCall<T> streamingCall,
        CancellationToken cancellationToken
    )
    {
        try
        {
            return await streamingCall.ResponseStream.MoveNext(cancellationToken);
        }
        catch (RpcException e) when (e.StatusCode == StatusCode.Cancelled)
        {
            return false;
        }
    }
}