using System.Runtime.CompilerServices;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Spravy.Client.Extensions;
using Spravy.Client.Interfaces;
using Spravy.Client.Services;
using Spravy.Domain.Enums;
using Spravy.Domain.Extensions;
using Spravy.Domain.Helpers;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.ToDo.Domain.Models;
using Spravy.ToDo.Protos;
using static Spravy.ToDo.Protos.ToDoService;

namespace Spravy.ToDo.Domain.Client.Services;

public class GrpcToDoService : GrpcServiceBase<ToDoServiceClient>,
    IToDoService,
    IGrpcServiceCreatorAuth<GrpcToDoService, ToDoServiceClient>
{
    public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(30);
    
    private readonly IConverter converter;
    private readonly IMetadataFactory metadataFactory;
    
    public GrpcToDoService(
        IFactory<Uri, ToDoServiceClient> grpcClientFactory,
        Uri host,
        IConverter converter,
        IMetadataFactory metadataFactory,
        ISerializer serializer
    ) : base(grpcClientFactory, host, serializer)
    {
        this.converter = converter;
        this.metadataFactory = metadataFactory;
    }
    
    public static GrpcToDoService CreateGrpcService(
        IFactory<Uri, ToDoServiceClient> grpcClientFactory,
        Uri host,
        IConverter mapper,
        IMetadataFactory metadataFactory,
        ISerializer serializer
    )
    {
        return new(grpcClientFactory, host, mapper, metadataFactory, serializer);
    }
    
    public ConfiguredValueTaskAwaitable<Result> CloneToDoItemAsync(
        Guid cloneId,
        Guid? parentId,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(converter.Convert<ByteString>(cloneId), converter.Convert<ByteString>(parentId),
                (metadata, ci, pi) => client.CloneToDoItemAsync(new()
                    {
                        CloneId = ci,
                        ParentId = pi,
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
           .IfSuccessAsync(converter.Convert<ByteString>(id), (metadata, i) => client
               .UpdateToDoItemDescriptionTypeAsync(
                    new()
                    {
                        Id = i,
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
               .IfSuccessAsync(converter.Convert<ByteString>(id), (metadata, i) => client
                   .GetReferenceToDoItemSettingsAsync(
                        new()
                        {
                            Id = i,
                        }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
                   .ToValueTaskResultValueOnly()
                   .ConfigureAwait(false), cancellationToken)
               .IfSuccessAsync(reply => converter.Convert<ReferenceToDoItemSettings>(reply), cancellationToken),
            cancellationToken);
    }
    
    public ConfiguredValueTaskAwaitable<Result> UpdateReferenceToDoItemAsync(
        Guid id,
        Guid referenceId,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(converter.Convert<ByteString>(id), converter.Convert<ByteString>(referenceId),
                (metadata, i, ri) => client.UpdateReferenceToDoItemAsync(new()
                    {
                        Id = i,
                        ReferenceId = ri,
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
               .IfSuccessAsync(converter.Convert<ResetToDoItemRequest>(options),
                    (metadata, request) =>
                        client.ResetToDoItemAsync(request, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
                           .ToValueTaskResultOnly()
                           .ConfigureAwait(false), cancellationToken), cancellationToken);
    }
    
    public ConfiguredValueTaskAwaitable<Result> RandomizeChildrenOrderIndexAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(converter.Convert<ByteString>(id), (metadata, i) => client.RandomizeChildrenOrderIndexAsync(
                    new()
                    {
                        Id = i,
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
           .IfSuccessAsync(converter.Convert<ByteString>(id), (metadata, i) => client.GetParentsAsync(new()
                {
                    Id = i,
                }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
               .ToValueTaskResultValueOnly()
               .ConfigureAwait(false)
               .IfSuccessAsync(
                    reply => converter.Convert<ToDoShortItem[]>(reply.Parents)
                       .IfSuccess(p => p.ToReadOnlyMemory().ToResult())
                       .ToValueTaskResult()
                       .ConfigureAwait(false), cancellationToken), cancellationToken), cancellationToken);
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
               .IfSuccessAsync(
                    reply => converter.Convert<Guid[]>(reply.Ids)
                       .IfSuccess(ids => ids.ToReadOnlyMemory().ToResult())
                       .ToValueTaskResult()
                       .ConfigureAwait(false), cancellationToken), cancellationToken), cancellationToken);
    }
    
    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetLeafToDoItemIdsAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(converter.Convert<ByteString>(id), (metadata, i) => client.GetLeafToDoItemIdsAsync(new()
                {
                    Id = i,
                }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
               .ToValueTaskResultValueOnly()
               .ConfigureAwait(false)
               .IfSuccessAsync(
                    reply => converter.Convert<Guid[]>(reply.Ids)
                       .IfSuccess(ids => ids.ToReadOnlyMemory().ToResult())
                       .ToValueTaskResult()
                       .ConfigureAwait(false), cancellationToken), cancellationToken), cancellationToken);
    }
    
    public ConfiguredValueTaskAwaitable<Result<ToDoItem>> GetToDoItemAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(converter.Convert<ByteString>(id), (metadata, i) => client.GetToDoItemAsync(new()
                {
                    Id = i,
                }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
               .ToValueTaskResultValueOnly()
               .ConfigureAwait(false)
               .IfSuccessAsync(reply => converter.Convert<ToDoItem>(reply).ToValueTaskResult().ConfigureAwait(false),
                    cancellationToken), cancellationToken), cancellationToken);
    }
    
    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetChildrenToDoItemIdsAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(converter.Convert<ByteString>(id), (metadata, i) => client.GetChildrenToDoItemIdsAsync(new()
                {
                    Id = i,
                }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
               .ToValueTaskResultValueOnly()
               .ConfigureAwait(false)
               .IfSuccessAsync(
                    reply => converter.Convert<Guid[]>(reply.Ids)
                       .IfSuccess(ids => ids.ToReadOnlyMemory().ToResult())
                       .ToValueTaskResult()
                       .ConfigureAwait(false), cancellationToken), cancellationToken), cancellationToken);
    }
    
    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<ToDoShortItem>>> GetChildrenToDoItemShortsAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(converter.Convert<ByteString>(id), (metadata, i) => client.GetChildrenToDoItemShortsAsync(
                    new()
                    {
                        Id = i,
                    }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
               .ToValueTaskResultValueOnly()
               .ConfigureAwait(false)
               .IfSuccessAsync(
                    reply => converter.Convert<ToDoShortItem[]>(reply.Items)
                       .IfSuccess(items => items.ToReadOnlyMemory().ToResult())
                       .ToValueTaskResult()
                       .ConfigureAwait(false), cancellationToken), cancellationToken), cancellationToken);
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
                       .IfSuccessAsync(
                            reply => converter.Convert<Guid[]>(reply.Ids)
                               .IfSuccess(ids => ids.ToReadOnlyMemory().ToResult())
                               .ToValueTaskResult()
                               .ConfigureAwait(false), cancellationToken), cancellationToken), cancellationToken);
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
                       .IfSuccessAsync(
                            reply => converter.Convert<Guid[]>(reply.Ids)
                               .IfSuccess(ids => ids.ToReadOnlyMemory().ToResult())
                               .ToValueTaskResult()
                               .ConfigureAwait(false), cancellationToken), cancellationToken), cancellationToken);
    }
    
    public ConfiguredValueTaskAwaitable<Result<Guid>> AddRootToDoItemAsync(
        AddRootToDoItemOptions options,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(cancellationToken)
               .IfSuccessAsync(converter.Convert<AddRootToDoItemRequest>(options),
                    (metadata, i) => client
                       .AddRootToDoItemAsync(i, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
                       .ToValueTaskResultValueOnly()
                       .ConfigureAwait(false)
                       .IfSuccessAsync(id => converter.Convert<Guid>(id.Id).ToValueTaskResult().ConfigureAwait(false),
                            cancellationToken), cancellationToken), cancellationToken);
    }
    
    public ConfiguredValueTaskAwaitable<Result<Guid>> AddToDoItemAsync(
        AddToDoItemOptions options,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(cancellationToken)
               .IfSuccessAsync(converter.Convert<AddToDoItemRequest>(options),
                    (metadata, request) => client
                       .AddToDoItemAsync(request, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
                       .ToValueTaskResultValueOnly()
                       .ConfigureAwait(false)
                       .IfSuccessAsync(id => converter.Convert<Guid>(id.Id).ToValueTaskResult().ConfigureAwait(false),
                            cancellationToken), cancellationToken), cancellationToken);
    }
    
    public ConfiguredValueTaskAwaitable<Result> DeleteToDoItemAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(converter.Convert<ByteString>(id), (metadata, i) => client.DeleteToDoItemAsync(new()
                {
                    Id = i,
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
           .IfSuccessAsync(converter.Convert<ByteString>(id), (metadata, i) => client
               .UpdateToDoItemTypeOfPeriodicityAsync(
                    new()
                    {
                        Id = i,
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
           .IfSuccessAsync(converter.Convert<ByteString>(id), converter.Convert<Timestamp>(dueDate),
                (metadata, i, dd) =>
                    client.UpdateToDoItemDueDateAsync(new()
                        {
                            Id = i,
                            DueDate = dd,
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
           .IfSuccessAsync(converter.Convert<ByteString>(id), (metadata, i) => client.UpdateToDoItemCompleteStatusAsync(
                    new()
                    {
                        Id = i,
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
           .IfSuccessAsync(converter.Convert<ByteString>(id), (metadata, i) => client.UpdateToDoItemNameAsync(new()
                {
                    Id = i,
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
               .IfSuccessAsync(converter.Convert<UpdateToDoItemOrderIndexRequest>(options),
                    (metadata, request) =>
                        client.UpdateToDoItemOrderIndexAsync(request, metadata, DateTime.UtcNow.Add(Timeout),
                                cancellationToken)
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
           .IfSuccessAsync(converter.Convert<ByteString>(id), (metadata, i) => client.UpdateToDoItemDescriptionAsync(
                    new()
                    {
                        Description = description,
                        Id = i,
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
           .IfSuccessAsync(converter.Convert<ByteString>(id), (metadata, i) => client.UpdateToDoItemTypeAsync(new()
                {
                    Id = i,
                    Type = (ToDoItemTypeGrpc)type,
                }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
               .ToValueTaskResultOnly()
               .ConfigureAwait(false), cancellationToken), cancellationToken);
    }
    
    public ConfiguredValueTaskAwaitable<Result> AddFavoriteToDoItemAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(converter.Convert<ByteString>(id), (metadata, i) => client.AddFavoriteToDoItemAsync(new()
                {
                    Id = i,
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
           .IfSuccessAsync(converter.Convert<ByteString>(id), (metadata, i) => client.RemoveFavoriteToDoItemAsync(new()
                {
                    Id = i,
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
           .IfSuccessAsync(converter.Convert<ByteString>(id), (metadata, i) => client
               .UpdateToDoItemIsRequiredCompleteInDueDateAsync(new()
                {
                    Id = i,
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
                       .IfSuccessAsync(
                            reply => converter.Convert<Guid[]>(reply.Ids)
                               .IfSuccess(ids => ids.ToReadOnlyMemory().ToResult())
                               .ToValueTaskResult()
                               .ConfigureAwait(false), cancellationToken), cancellationToken), cancellationToken);
    }
    
    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemAnnuallyPeriodicityAsync(
        Guid id,
        AnnuallyPeriodicity periodicity,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(converter.Convert<ByteString>(id), converter.Convert<AnnuallyPeriodicityGrpc>(periodicity),
                (value, i, p) => client.UpdateToDoItemAnnuallyPeriodicityAsync(new()
                    {
                        Periodicity = p,
                        Id = i,
                    }, value, DateTime.UtcNow.Add(Timeout), cancellationToken)
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
           .IfSuccessAsync(converter.Convert<ByteString>(id), converter.Convert<MonthlyPeriodicityGrpc>(periodicity),
                (metadata, i, p) => client.UpdateToDoItemMonthlyPeriodicityAsync(new()
                    {
                        Periodicity = p,
                        Id = i,
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
           .IfSuccessAsync(converter.Convert<ByteString>(id), converter.Convert<WeeklyPeriodicityGrpc>(periodicity),
                (metadata, i, p) => client.UpdateToDoItemWeeklyPeriodicityAsync(new()
                    {
                        Periodicity = p,
                        Id = i,
                    }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
                   .ToValueTaskResultOnly()
                   .ConfigureAwait(false), cancellationToken), cancellationToken);
    }
    
    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<ToDoSelectorItem>>> GetToDoSelectorItemsAsync(
        Guid[] ignoreIds,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(converter.Convert<ByteString[]>(ignoreIds), (metadata, ii) =>
            {
                var request = new GetToDoSelectorItemsRequest();
                request.IgnoreIds.AddRange(ii);
                
                return client
                   .GetToDoSelectorItemsAsync(request, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
                   .ToValueTaskResultValueOnly()
                   .ConfigureAwait(false)
                   .IfSuccessAsync(
                        reply => converter.Convert<ToDoSelectorItem[]>(reply.Items)
                           .IfSuccess(items => items.ToReadOnlyMemory().ToResult())
                           .ToValueTaskResult()
                           .ConfigureAwait(false), cancellationToken);
            }, cancellationToken), cancellationToken);
    }
    
    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemParentAsync(
        Guid id,
        Guid parentId,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(converter.Convert<ByteString>(id), converter.Convert<ByteString>(parentId),
                (metadata, i, pi) =>
                    client.UpdateToDoItemParentAsync(new()
                        {
                            Id = i,
                            ParentId = pi,
                        }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
                       .ToValueTaskResultOnly()
                       .ConfigureAwait(false), cancellationToken), cancellationToken);
    }
    
    public ConfiguredValueTaskAwaitable<Result> ToDoItemToRootAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(converter.Convert<ByteString>(id), (metadata, i) => client.ToDoItemToRootAsync(new()
                {
                    Id = i,
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
               .IfSuccessAsync(converter.Convert<ToDoItemToStringRequest>(options),
                    (metadata, request) => client
                       .ToDoItemToStringAsync(request, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
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
           .IfSuccessAsync(converter.Convert<ByteString>(id), (metadata, i) => client.UpdateToDoItemDaysOffsetAsync(
                    new()
                    {
                        Id = i,
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
           .IfSuccessAsync(converter.Convert<ByteString>(id), (metadata, i) => client.UpdateToDoItemMonthsOffsetAsync(
                    new()
                    {
                        Id = i,
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
           .IfSuccessAsync(converter.Convert<ByteString>(id), (metadata, i) => client.UpdateToDoItemWeeksOffsetAsync(
                    new()
                    {
                        Id = i,
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
           .IfSuccessAsync(converter.Convert<ByteString>(id), (metadata, i) => client.UpdateToDoItemYearsOffsetAsync(
                    new()
                    {
                        Id = i,
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
           .IfSuccessAsync(converter.Convert<ByteString>(id), (metadata, i) => client.UpdateToDoItemChildrenTypeAsync(
                    new()
                    {
                        Id = i,
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
           .IfSuccessAsync(converter.Convert<ByteString>(id), (metadata, i) => client.GetSiblingsAsync(new()
                {
                    Id = i,
                }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
               .ToValueTaskResultValueOnly()
               .ConfigureAwait(false)
               .IfSuccessAsync(
                    items => converter.Convert<ToDoShortItem[]>(items.Items)
                       .IfSuccess(it => it.ToReadOnlyMemory().ToResult())
                       .ToValueTaskResult()
                       .ConfigureAwait(false), cancellationToken), cancellationToken), cancellationToken);
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
                       .IfSuccessAsync(
                            reply => converter.Convert<OptionStruct<ActiveToDoItem>>(reply.Item)
                               .ToValueTaskResult()
                               .ConfigureAwait(false), cancellationToken), cancellationToken), cancellationToken);
    }
    
    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemLinkAsync(
        Guid id,
        Uri? link,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(converter.Convert<OptionString>(link), converter.Convert<ByteString>(id), (metadata, l, i) =>
                client.UpdateToDoItemLinkAsync(new()
                    {
                        Id = i,
                        Link = l.Value,
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
           .IfSuccessAsync(converter.Convert<ByteString>(id), (metadata, i) => client.GetPlannedToDoItemSettingsAsync(
                    new()
                    {
                        Id = i,
                    }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
               .ToValueTaskResultValueOnly()
               .ConfigureAwait(false)
               .IfSuccessAsync(
                    reply => converter.Convert<PlannedToDoItemSettings>(reply)
                       .ToValueTaskResult()
                       .ConfigureAwait(false),
                    cancellationToken), cancellationToken), cancellationToken);
    }
    
    public ConfiguredValueTaskAwaitable<Result<ValueToDoItemSettings>> GetValueToDoItemSettingsAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(converter.Convert<ByteString>(id), (metadata, i) => client.GetValueToDoItemSettingsAsync(
                    new()
                    {
                        Id = i,
                    }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
               .ToValueTaskResultValueOnly()
               .ConfigureAwait(false)
               .IfSuccessAsync(
                    reply => converter.Convert<ValueToDoItemSettings>(reply).ToValueTaskResult().ConfigureAwait(false),
                    cancellationToken), cancellationToken), cancellationToken);
    }
    
    public ConfiguredValueTaskAwaitable<Result<PeriodicityToDoItemSettings>> GetPeriodicityToDoItemSettingsAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(converter.Convert<ByteString>(id), (metadata, i) => client
               .GetPeriodicityToDoItemSettingsAsync(
                    new()
                    {
                        Id = i,
                    }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
               .ToValueTaskResultValueOnly()
               .ConfigureAwait(false)
               .IfSuccessAsync(
                    reply => converter.Convert<PeriodicityToDoItemSettings>(reply)
                       .ToValueTaskResult()
                       .ConfigureAwait(false),
                    cancellationToken), cancellationToken), cancellationToken);
    }
    
    public ConfiguredValueTaskAwaitable<Result<WeeklyPeriodicity>> GetWeeklyPeriodicityAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(converter.Convert<ByteString>(id), (metadata, i) => client.GetWeeklyPeriodicityAsync(new()
                {
                    Id = i,
                }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
               .ToValueTaskResultValueOnly()
               .ConfigureAwait(false)
               .IfSuccessAsync(
                    reply => converter.Convert<WeeklyPeriodicity>(reply).ToValueTaskResult().ConfigureAwait(false),
                    cancellationToken), cancellationToken), cancellationToken);
    }
    
    public ConfiguredValueTaskAwaitable<Result<MonthlyPeriodicity>> GetMonthlyPeriodicityAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(converter.Convert<ByteString>(id), (metadata, i) => client.GetMonthlyPeriodicityAsync(new()
                {
                    Id = i,
                }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
               .ToValueTaskResultValueOnly()
               .ConfigureAwait(false)
               .IfSuccessAsync(
                    reply => converter.Convert<MonthlyPeriodicity>(reply).ToValueTaskResult().ConfigureAwait(false),
                    cancellationToken), cancellationToken), cancellationToken);
    }
    
    public ConfiguredValueTaskAwaitable<Result<AnnuallyPeriodicity>> GetAnnuallyPeriodicityAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(converter.Convert<ByteString>(id), (metadata, i) => client.GetAnnuallyPeriodicityAsync(new()
                {
                    Id = i,
                }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
               .ToValueTaskResultValueOnly()
               .ConfigureAwait(false)
               .IfSuccessAsync(
                    reply => converter.Convert<AnnuallyPeriodicity>(reply).ToValueTaskResult().ConfigureAwait(false),
                    cancellationToken), cancellationToken), cancellationToken);
    }
    
    public ConfiguredValueTaskAwaitable<Result<PeriodicityOffsetToDoItemSettings>>
        GetPeriodicityOffsetToDoItemSettingsAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(converter.Convert<ByteString>(id), (metadata, i) => client
               .GetPeriodicityOffsetToDoItemSettingsAsync(new()
                {
                    Id = i,
                }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
               .ToValueTaskResultValueOnly()
               .ConfigureAwait(false)
               .IfSuccessAsync(
                    reply => converter.Convert<PeriodicityOffsetToDoItemSettings>(reply)
                       .ToValueTaskResult()
                       .ConfigureAwait(false), cancellationToken), cancellationToken), cancellationToken);
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
        ToDoServiceClient client,
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
        
        var idsByteString = converter.Convert<ByteString[]>(ids.ToArray());
        
        if (idsByteString.IsHasError)
        {
            yield return new(idsByteString.Errors);
            
            yield break;
        }
        
        request.Ids.AddRange(idsByteString.Value);
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
            
            var item = converter.Convert<ToDoItem[]>(reply.Items.ToArray())
               .IfSuccess(x => x.ToReadOnlyMemory().ToResult());
            
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