using System.Runtime.CompilerServices;
using Grpc.Core;
using Spravy.Client.Extensions;
using Spravy.Client.Helpers;
using Spravy.Client.Interfaces;
using Spravy.Client.Services;
using Spravy.Core.Interfaces;
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

public class GrpcToDoService
    : GrpcServiceBase<ToDoService.ToDoServiceClient>,
        IToDoService,
        IGrpcServiceCreatorAuth<GrpcToDoService, ToDoService.ToDoServiceClient>
{
    public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(30);

    private readonly IMetadataFactory metadataFactory;

    public GrpcToDoService(
        IFactory<Uri, ToDoService.ToDoServiceClient> grpcClientFactory,
        Uri host,
        IMetadataFactory metadataFactory,
        IRpcExceptionHandler handler
    )
        : base(grpcClientFactory, host, handler)
    {
        this.metadataFactory = metadataFactory;
    }

    public static GrpcToDoService CreateGrpcService(
        IFactory<Uri, ToDoService.ToDoServiceClient> grpcClientFactory,
        Uri host,
        IMetadataFactory metadataFactory,
        IRpcExceptionHandler handler
    )
    {
        return new(grpcClientFactory, host, metadataFactory, handler);
    }

    public ConfiguredValueTaskAwaitable<
        Result<OptionStruct<ActiveToDoItem>>
    > GetActiveToDoItemAsync(Guid id, CancellationToken ct)
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .GetActiveToDoItem(
                                    new() { Id = id.ToByteString(), },
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToResult(),
                        ct
                    )
                    .IfSuccessAsync(reply => reply.Item.ToOptionActiveToDoItem().ToResult(), ct),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result<Guid>> CloneToDoItemAsync(
        Guid cloneId,
        OptionStruct<Guid> parentId,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .CloneToDoItem(
                                    new()
                                    {
                                        CloneId = cloneId.ToByteString(),
                                        ParentId = parentId.ToByteString(),
                                    },
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToResult()
                                .IfSuccess(reply => reply.NewItemId.ToGuid().ToResult()),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemDescriptionTypeAsync(
        Guid id,
        DescriptionType type,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .UpdateToDoItemDescriptionTypeAsync(
                                    new()
                                    {
                                        Id = id.ToByteString(),
                                        Type = (DescriptionTypeGrpc)type,
                                    },
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateReferenceToDoItemAsync(
        Guid id,
        Guid referenceId,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .UpdateReferenceToDoItemAsync(
                                    new()
                                    {
                                        Id = id.ToByteString(),
                                        ReferenceId = referenceId.ToByteString(),
                                    },
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result> ResetToDoItemAsync(
        ResetToDoItemOptions options,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .ResetToDoItemAsync(
                                    options.ToResetToDoItemRequest(),
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result> RandomizeChildrenOrderIndexAsync(
        Guid id,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .RandomizeChildrenOrderIndexAsync(
                                    new() { Id = id.ToByteString(), },
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<ToDoShortItem>>> GetParentsAsync(
        Guid id,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .GetParentsAsync(
                                    new() { Id = id.ToByteString(), },
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(
                                    reply => reply.Parents.ToToDoShortItem().ToResult(),
                                    ct
                                ),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> SearchToDoItemIdsAsync(
        string searchText,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .SearchToDoItemIdsAsync(
                                    new() { SearchText = searchText, },
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(reply => reply.Ids.ToGuid().ToResult(), ct),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetLeafToDoItemIdsAsync(
        Guid id,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .GetLeafToDoItemIdsAsync(
                                    new() { Id = id.ToByteString(), },
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(reply => reply.Ids.ToGuid().ToResult(), ct),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result<ToDoItem>> GetToDoItemAsync(
        Guid id,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .GetToDoItemAsync(
                                    new() { Id = id.ToByteString(), },
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(reply => reply.ToToDoItem().ToResult(), ct),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetChildrenToDoItemIdsAsync(
        Guid id,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .GetChildrenToDoItemIdsAsync(
                                    new() { Id = id.ToByteString(), },
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(reply => reply.Ids.ToGuid().ToResult(), ct),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<
        Result<ReadOnlyMemory<ToDoShortItem>>
    > GetChildrenToDoItemShortsAsync(Guid id, CancellationToken ct)
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .GetChildrenToDoItemShortsAsync(
                                    new() { Id = id.ToByteString(), },
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(
                                    reply => reply.Items.ToToDoShortItem().ToResult(),
                                    ct
                                ),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetRootToDoItemIdsAsync(
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .GetRootToDoItemIdsAsync(
                                    new(),
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(reply => reply.Ids.ToGuid().ToResult(), ct),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetFavoriteToDoItemIdsAsync(
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .GetFavoriteToDoItemIdsAsync(
                                    new(),
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(reply => reply.Ids.ToGuid().ToResult(), ct),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result<Guid>> AddRootToDoItemAsync(
        AddRootToDoItemOptions options,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .AddRootToDoItemAsync(
                                    options.ToAddRootToDoItemRequest(),
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(id => id.Id.ToGuid().ToResult(), ct),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result<Guid>> AddToDoItemAsync(
        AddToDoItemOptions options,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .AddToDoItemAsync(
                                    options.ToAddToDoItemRequest(),
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(id => id.Id.ToGuid().ToResult(), ct),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result> DeleteToDoItemAsync(Guid id, CancellationToken ct)
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .DeleteToDoItemAsync(
                                    new() { Id = id.ToByteString(), },
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemTypeOfPeriodicityAsync(
        Guid id,
        TypeOfPeriodicity type,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .UpdateToDoItemTypeOfPeriodicityAsync(
                                    new()
                                    {
                                        Id = id.ToByteString(),
                                        Type = (TypeOfPeriodicityGrpc)type,
                                    },
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemDueDateAsync(
        Guid id,
        DateOnly dueDate,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .UpdateToDoItemDueDateAsync(
                                    new()
                                    {
                                        Id = id.ToByteString(),
                                        DueDate = dueDate.ToTimestamp(),
                                    },
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemCompleteStatusAsync(
        Guid id,
        bool isComplete,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(CancellationToken.None)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .UpdateToDoItemCompleteStatusAsync(
                                    new() { Id = id.ToByteString(), IsCompleted = isComplete, },
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    CancellationToken.None
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false),
                        CancellationToken.None
                    ),
            CancellationToken.None
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemNameAsync(
        Guid id,
        string name,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .UpdateToDoItemNameAsync(
                                    new() { Id = id.ToByteString(), Name = name, },
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemOrderIndexAsync(
        UpdateOrderIndexToDoItemOptions options,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .UpdateToDoItemOrderIndexAsync(
                                    options.ToUpdateToDoItemOrderIndexRequest(),
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemDescriptionAsync(
        Guid id,
        string description,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .UpdateToDoItemDescriptionAsync(
                                    new() { Description = description, Id = id.ToByteString(), },
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemTypeAsync(
        Guid id,
        ToDoItemType type,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .UpdateToDoItemTypeAsync(
                                    new()
                                    {
                                        Id = id.ToByteString(),
                                        Type = (ToDoItemTypeGrpc)type,
                                    },
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result> AddFavoriteToDoItemAsync(
        Guid id,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .AddFavoriteToDoItemAsync(
                                    new() { Id = id.ToByteString(), },
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result> RemoveFavoriteToDoItemAsync(
        Guid id,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .RemoveFavoriteToDoItemAsync(
                                    new() { Id = id.ToByteString(), },
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemIsRequiredCompleteInDueDateAsync(
        Guid id,
        bool value,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .UpdateToDoItemIsRequiredCompleteInDueDateAsync(
                                    new()
                                    {
                                        Id = id.ToByteString(),
                                        IsRequiredCompleteInDueDate = value,
                                    },
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetTodayToDoItemsAsync(
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .GetTodayToDoItemsAsync(
                                    new(),
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(reply => reply.Ids.ToGuid().ToResult(), ct),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemAnnuallyPeriodicityAsync(
        Guid id,
        AnnuallyPeriodicity periodicity,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .UpdateToDoItemAnnuallyPeriodicityAsync(
                                    new()
                                    {
                                        Periodicity = periodicity.ToAnnuallyPeriodicityGrpc(),
                                        Id = id.ToByteString(),
                                    },
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemMonthlyPeriodicityAsync(
        Guid id,
        MonthlyPeriodicity periodicity,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .UpdateToDoItemMonthlyPeriodicityAsync(
                                    new()
                                    {
                                        Periodicity = periodicity.ToMonthlyPeriodicityGrpc(),
                                        Id = id.ToByteString(),
                                    },
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemWeeklyPeriodicityAsync(
        Guid id,
        WeeklyPeriodicity periodicity,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .UpdateToDoItemWeeklyPeriodicityAsync(
                                    new()
                                    {
                                        Periodicity = periodicity.ToWeeklyPeriodicityGrpc(),
                                        Id = id.ToByteString(),
                                    },
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<
        Result<ReadOnlyMemory<ToDoSelectorItem>>
    > GetToDoSelectorItemsAsync(ReadOnlyMemory<Guid> ignoreIds, CancellationToken ct)
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                        {
                            var request = new GetToDoSelectorItemsRequest();
                            request.IgnoreIds.AddRange(ignoreIds.ToByteString().ToArray());

                            return client
                                .GetToDoSelectorItemsAsync(
                                    request,
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(
                                    reply => reply.Items.ToToDoSelectorItem().ToResult(),
                                    ct
                                );
                        },
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemParentAsync(
        Guid id,
        Guid parentId,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .UpdateToDoItemParentAsync(
                                    new()
                                    {
                                        Id = id.ToByteString(),
                                        ParentId = parentId.ToByteString(),
                                    },
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result> ToDoItemToRootAsync(Guid id, CancellationToken ct)
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .ToDoItemToRootAsync(
                                    new() { Id = id.ToByteString(), },
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result<string>> ToDoItemToStringAsync(
        ToDoItemToStringOptions options,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .ToDoItemToStringAsync(
                                    options.ToToDoItemToStringRequest(),
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(
                                    reply =>
                                        reply
                                            .Value.ToResult()
                                            .ToValueTaskResult()
                                            .ConfigureAwait(false),
                                    ct
                                ),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemDaysOffsetAsync(
        Guid id,
        ushort days,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .UpdateToDoItemDaysOffsetAsync(
                                    new() { Id = id.ToByteString(), Days = days, },
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemMonthsOffsetAsync(
        Guid id,
        ushort months,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .UpdateToDoItemMonthsOffsetAsync(
                                    new() { Id = id.ToByteString(), Months = months, },
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemWeeksOffsetAsync(
        Guid id,
        ushort weeks,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .UpdateToDoItemWeeksOffsetAsync(
                                    new() { Id = id.ToByteString(), Weeks = weeks, },
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemYearsOffsetAsync(
        Guid id,
        ushort years,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .UpdateToDoItemYearsOffsetAsync(
                                    new() { Id = id.ToByteString(), Years = years, },
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemChildrenTypeAsync(
        Guid id,
        ToDoItemChildrenType type,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .UpdateToDoItemChildrenTypeAsync(
                                    new()
                                    {
                                        Id = id.ToByteString(),
                                        Type = (ToDoItemChildrenTypeGrpc)type,
                                    },
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<ToDoShortItem>>> GetSiblingsAsync(
        Guid id,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .GetSiblingsAsync(
                                    new() { Id = id.ToByteString(), },
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(
                                    items => items.Items.ToToDoShortItem().ToResult(),
                                    ct
                                ),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<
        Result<OptionStruct<ActiveToDoItem>>
    > GetCurrentActiveToDoItemAsync(CancellationToken ct)
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .GetCurrentActiveToDoItemAsync(
                                    DefaultObject<GetCurrentActiveToDoItemRequest>.Default,
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(
                                    reply => reply.Item.ToOptionActiveToDoItem().ToResult(),
                                    ct
                                ),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemLinkAsync(
        Guid id,
        Option<Uri> link,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .UpdateToDoItemLinkAsync(
                                    new() { Id = id.ToByteString(), Link = link.MapToString(), },
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<
        Result<PlannedToDoItemSettings>
    > GetPlannedToDoItemSettingsAsync(Guid id, CancellationToken ct)
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .GetPlannedToDoItemSettingsAsync(
                                    new() { Id = id.ToByteString(), },
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(
                                    reply => reply.ToPlannedToDoItemSettings().ToResult(),
                                    ct
                                ),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<
        Result<ValueToDoItemSettings>
    > GetValueToDoItemSettingsAsync(Guid id, CancellationToken ct)
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .GetValueToDoItemSettingsAsync(
                                    new() { Id = id.ToByteString(), },
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(
                                    reply => reply.ToValueToDoItemSettings().ToResult(),
                                    ct
                                ),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<
        Result<PeriodicityToDoItemSettings>
    > GetPeriodicityToDoItemSettingsAsync(Guid id, CancellationToken ct)
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .GetPeriodicityToDoItemSettingsAsync(
                                    new() { Id = id.ToByteString(), },
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(
                                    reply => reply.ToPeriodicityToDoItemSettings().ToResult(),
                                    ct
                                ),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result<WeeklyPeriodicity>> GetWeeklyPeriodicityAsync(
        Guid id,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .GetWeeklyPeriodicityAsync(
                                    new() { Id = id.ToByteString(), },
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(
                                    reply => reply.ToWeeklyPeriodicity().ToResult(),
                                    ct
                                ),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result<MonthlyPeriodicity>> GetMonthlyPeriodicityAsync(
        Guid id,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .GetMonthlyPeriodicityAsync(
                                    new() { Id = id.ToByteString(), },
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(
                                    reply => reply.ToMonthlyPeriodicity().ToResult(),
                                    ct
                                ),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result<AnnuallyPeriodicity>> GetAnnuallyPeriodicityAsync(
        Guid id,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .GetAnnuallyPeriodicityAsync(
                                    new() { Id = id.ToByteString(), },
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(
                                    reply => reply.ToAnnuallyPeriodicity().ToResult(),
                                    ct
                                ),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<
        Result<PeriodicityOffsetToDoItemSettings>
    > GetPeriodicityOffsetToDoItemSettingsAsync(Guid id, CancellationToken ct)
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .GetPeriodicityOffsetToDoItemSettingsAsync(
                                    new() { Id = id.ToByteString(), },
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(
                                    reply => reply.ToPeriodicityOffsetToDoItemSettings().ToResult(),
                                    ct
                                ),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredCancelableAsyncEnumerable<Result<ReadOnlyMemory<ToDoItem>>> GetToDoItemsAsync(
        ReadOnlyMemory<Guid> ids,
        uint chunkSize,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            (client, token) =>
                GetToDoItemsCore(client, ids, chunkSize, token).ConfigureAwait(false),
            ct
        );
    }

    private async IAsyncEnumerable<Result<ReadOnlyMemory<ToDoItem>>> GetToDoItemsCore(
        ToDoService.ToDoServiceClient client,
        ReadOnlyMemory<Guid> ids,
        uint chunkSize,
        [EnumeratorCancellation] CancellationToken ct
    )
    {
        if (ids.IsEmpty)
        {
            yield return ReadOnlyMemory<ToDoItem>.Empty.ToResult();

            yield break;
        }

        var request = new GetToDoItemsRequest { ChunkSize = chunkSize, };

        var idsByteString = ids.ToByteString();
        request.Ids.AddRange(idsByteString.ToArray());
        var metadata = await metadataFactory.CreateAsync(ct);

        if (!metadata.TryGetValue(out var value))
        {
            yield return new(metadata.Errors);

            yield break;
        }

        using var response = client.GetToDoItems(request, value, DateTime.UtcNow.Add(Timeout), ct);

        while (await MoveNextAsync(response, ct))
        {
            var reply = response.ResponseStream.Current;
            var item = reply.Items.ToToDoItem().ToResult();

            yield return item;
        }
    }

    private async ValueTask<bool> MoveNextAsync<T>(
        AsyncServerStreamingCall<T> streamingCall,
        CancellationToken ct
    )
    {
        try
        {
            return await streamingCall.ResponseStream.MoveNext(ct);
        }
        catch (RpcException e) when (e.StatusCode == StatusCode.Cancelled)
        {
            return false;
        }
    }
}
