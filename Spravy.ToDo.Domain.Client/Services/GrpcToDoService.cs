using Spravy.Domain.Helpers;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Mapper.Mappers;
using Spravy.ToDo.Domain.Models;

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
        IRpcExceptionHandler handler,
        IRetryService retryService
    )
        : base(grpcClientFactory, host, handler, retryService)
    {
        this.metadataFactory = metadataFactory;
    }

    public static GrpcToDoService CreateGrpcService(
        IFactory<Uri, ToDoService.ToDoServiceClient> grpcClientFactory,
        Uri host,
        IMetadataFactory metadataFactory,
        IRpcExceptionHandler handler,
        IRetryService retryService
    )
    {
        return new(grpcClientFactory, host, metadataFactory, handler, retryService);
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
                                    new() { Id = id.ToByteString() },
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

    public Cvtar UpdateToDoItemDescriptionTypeAsync(
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

    public Cvtar UpdateReferenceToDoItemAsync(Guid id, Guid referenceId, CancellationToken ct)
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

    public Cvtar ResetToDoItemAsync(ResetToDoItemOptions options, CancellationToken ct)
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

    public Cvtar UpdateIconAsync(Guid id, string icon, CancellationToken ct)
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .UpdateIconAsync(
                                    new() { Id = id.ToByteString(), Icon = icon, },
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

    public ConfiguredValueTaskAwaitable<Result<bool>> UpdateEventsAsync(CancellationToken ct)
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .UpdateEventsAsync(new(), metadata, cancellationToken: ct)
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(reply => reply.IsUpdated.ToResult(), ct),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetBookmarkToDoItemIdsAsync(
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
                                .GetBookmarkToDoItemIdsAsync(
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

    public Cvtar UpdateIsBookmarkAsync(Guid id, bool isBookmark, CancellationToken ct)
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .UpdateIsBookmarkAsync(
                                    new() { Id = id.ToByteString(), IsBookmark = isBookmark },
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

    public Cvtar RandomizeChildrenOrderIndexAsync(Guid id, CancellationToken ct)
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .RandomizeChildrenOrderIndexAsync(
                                    new() { Id = id.ToByteString() },
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
                                    new() { Id = id.ToByteString() },
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
                                    new() { SearchText = searchText },
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
                                    new() { Id = id.ToByteString() },
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

    public ConfiguredValueTaskAwaitable<Result<FullToDoItem>> GetToDoItemAsync(
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
                                    new() { Id = id.ToByteString() },
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(reply => reply.ToFullToDoItem().ToResult(), ct),
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
                                    new() { Id = id.ToByteString() },
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
                                    new() { Id = id.ToByteString() },
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

    public Cvtar DeleteToDoItemAsync(Guid id, CancellationToken ct)
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .DeleteToDoItemAsync(
                                    new() { Id = id.ToByteString() },
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

    public Cvtar UpdateToDoItemTypeOfPeriodicityAsync(
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

    public Cvtar UpdateToDoItemDueDateAsync(Guid id, DateOnly dueDate, CancellationToken ct)
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

    public Cvtar UpdateToDoItemCompleteStatusAsync(Guid id, bool isComplete, CancellationToken ct)
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(CancellationToken.None)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .UpdateToDoItemCompleteStatusAsync(
                                    new() { Id = id.ToByteString(), IsCompleted = isComplete },
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

    public Cvtar UpdateToDoItemNameAsync(Guid id, string name, CancellationToken ct)
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .UpdateToDoItemNameAsync(
                                    new() { Id = id.ToByteString(), Name = name },
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

    public Cvtar UpdateToDoItemOrderIndexAsync(
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

    public Cvtar UpdateToDoItemDescriptionAsync(Guid id, string description, CancellationToken ct)
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .UpdateToDoItemDescriptionAsync(
                                    new() { Description = description, Id = id.ToByteString() },
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

    public Cvtar UpdateToDoItemTypeAsync(Guid id, ToDoItemType type, CancellationToken ct)
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .UpdateToDoItemTypeAsync(
                                    new() { Id = id.ToByteString(), Type = (ToDoItemTypeGrpc)type },
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

    public Cvtar AddFavoriteToDoItemAsync(Guid id, CancellationToken ct)
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .AddFavoriteToDoItemAsync(
                                    new() { Id = id.ToByteString() },
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

    public Cvtar RemoveFavoriteToDoItemAsync(Guid id, CancellationToken ct)
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .RemoveFavoriteToDoItemAsync(
                                    new() { Id = id.ToByteString() },
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

    public Cvtar UpdateToDoItemIsRequiredCompleteInDueDateAsync(
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

    public Cvtar UpdateToDoItemAnnuallyPeriodicityAsync(
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

    public Cvtar UpdateToDoItemMonthlyPeriodicityAsync(
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

    public Cvtar UpdateToDoItemWeeklyPeriodicityAsync(
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

    public Cvtar UpdateToDoItemParentAsync(Guid id, Guid parentId, CancellationToken ct)
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

    public Cvtar ToDoItemToRootAsync(Guid id, CancellationToken ct)
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .ToDoItemToRootAsync(
                                    new() { Id = id.ToByteString() },
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

    public Cvtar UpdateToDoItemDaysOffsetAsync(Guid id, ushort days, CancellationToken ct)
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .UpdateToDoItemDaysOffsetAsync(
                                    new() { Id = id.ToByteString(), Days = days },
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

    public Cvtar UpdateToDoItemMonthsOffsetAsync(Guid id, ushort months, CancellationToken ct)
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .UpdateToDoItemMonthsOffsetAsync(
                                    new() { Id = id.ToByteString(), Months = months },
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

    public Cvtar UpdateToDoItemWeeksOffsetAsync(Guid id, ushort weeks, CancellationToken ct)
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .UpdateToDoItemWeeksOffsetAsync(
                                    new() { Id = id.ToByteString(), Weeks = weeks },
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

    public Cvtar UpdateToDoItemYearsOffsetAsync(Guid id, ushort years, CancellationToken ct)
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .UpdateToDoItemYearsOffsetAsync(
                                    new() { Id = id.ToByteString(), Years = years },
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

    public Cvtar UpdateToDoItemChildrenTypeAsync(
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
                                    new() { Id = id.ToByteString() },
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

    public Cvtar UpdateToDoItemLinkAsync(Guid id, Option<Uri> link, CancellationToken ct)
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .UpdateToDoItemLinkAsync(
                                    new() { Id = id.ToByteString(), Link = link.MapToString() },
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
                                    new() { Id = id.ToByteString() },
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
                                    new() { Id = id.ToByteString() },
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
                                    new() { Id = id.ToByteString() },
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
                                    new() { Id = id.ToByteString() },
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
                                    new() { Id = id.ToByteString() },
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
                                    new() { Id = id.ToByteString() },
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
                                    new() { Id = id.ToByteString() },
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

        var request = new GetToDoItemsRequest { ChunkSize = chunkSize };

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
