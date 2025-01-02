using Spravy.Core.Mappers;
using Spravy.ToDo.Domain.Mapper.Mappers;
using Spravy.ToDo.Domain.Models;

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
        IRpcExceptionHandler handler,
        IRetryService retryService
    ) : base(grpcClientFactory, host, handler, retryService)
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
        return new(
            grpcClientFactory,
            host,
            metadataFactory,
            handler,
            retryService
        );
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> CloneToDoItemAsync(
        ReadOnlyMemory<Guid> cloneIds,
        OptionStruct<Guid> parentId,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(ct)
               .IfSuccessAsync(
                    metadata =>
                    {
                        var request = new CloneToDoItemRequest
                        {
                            ParentId = parentId.ToByteString(),
                        };

                        request.CloneIds.AddRange(cloneIds.Select(x => x.ToByteString()).ToArray());

                        return client.CloneToDoItemAsync(request, metadata, DateTime.UtcNow.Add(Timeout), ct)
                           .ToValueTaskResultValueOnly()
                           .ConfigureAwait(false)
                           .IfSuccessAsync(
                                reply => reply.NewItemIds
                                   .Select(x => x.ToGuid())
                                   .ToArray()
                                   .ToReadOnlyMemory()
                                   .ToResult(),
                                ct
                            );
                    },
                    ct
                ),
            ct
        );
    }

    public Cvtar ResetToDoItemAsync(ReadOnlyMemory<ResetToDoItemOptions> options, CancellationToken ct)
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(ct)
               .IfSuccessAsync(
                    metadata =>
                    {
                        var request = new ResetToDoItemRequest();
                        request.Items.AddRange(options.Select(x => x.ToResetToDoItemOptionsGrpc()).ToArray());

                        return client.ResetToDoItemAsync(request, metadata, DateTime.UtcNow.Add(Timeout), ct)
                           .ToValueTaskResultOnly()
                           .ConfigureAwait(false);
                    },
                    ct
                ),
            ct
        );
    }

    public Cvtar EditToDoItemsAsync(EditToDoItems options, CancellationToken ct)
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(ct)
               .IfSuccessAsync(
                    metadata => client.EditToDoItemsAsync(
                            new()
                            {
                                Value = options.ToEditToDoItemsGrpc(),
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

    public ConfiguredValueTaskAwaitable<Result<bool>> UpdateEventsAsync(CancellationToken ct)
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(ct)
               .IfSuccessAsync(
                    metadata => client.UpdateEventsAsync(new(), metadata, cancellationToken: ct)
                       .ToValueTaskResultValueOnly()
                       .ConfigureAwait(false)
                       .IfSuccessAsync(reply => reply.IsUpdated.ToResult(), ct),
                    ct
                ),
            ct
        );
    }

    public Cvtar SwitchCompleteAsync(ReadOnlyMemory<Guid> ids, CancellationToken ct)
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(ct)
               .IfSuccessAsync(
                    metadata =>
                    {
                        var request = new SwitchCompleteRequest();
                        request.Ids.AddRange(ids.Select(x => x.ToByteString()).ToArray());

                        return client.SwitchCompleteAsync(request, metadata, DateTime.UtcNow.Add(Timeout), ct)
                           .ToValueTaskResultOnly()
                           .ConfigureAwait(false);
                    },
                    ct
                ),
            ct
        );
    }

    public Cvtar UpdateToDoItemOrderIndexAsync(
        ReadOnlyMemory<UpdateOrderIndexToDoItemOptions> options,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(ct)
               .IfSuccessAsync(
                    metadata =>
                    {
                        var request = new UpdateToDoItemOrderIndexRequest();

                        request.Items.AddRange(
                            options.Select(x => x.ToUpdateOrderIndexToDoItemOptionsGrpc()).ToArray()
                        );

                        return client.UpdateToDoItemOrderIndexAsync(request, metadata, DateTime.UtcNow.Add(Timeout), ct)
                           .ToValueTaskResultOnly()
                           .ConfigureAwait(false);
                    },
                    ct
                ),
            ct
        );
    }

    public Cvtar RandomizeChildrenOrderIndexAsync(ReadOnlyMemory<Guid> ids, CancellationToken ct)
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(ct)
               .IfSuccessAsync(
                    metadata =>
                    {
                        var request = new RandomizeChildrenOrderIndexRequest();
                        request.Ids.AddRange(ids.Select(x => x.ToByteString()).ToArray());

                        return client.RandomizeChildrenOrderIndexAsync(
                                request,
                                metadata,
                                DateTime.UtcNow.Add(Timeout),
                                ct
                            )
                           .ToValueTaskResultOnly()
                           .ConfigureAwait(false);
                    },
                    ct
                ),
            ct
        );
    }
    
    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> AddToDoItemAsync(
        ReadOnlyMemory<AddToDoItemOptions> options,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(ct)
               .IfSuccessAsync(
                    metadata =>
                    {
                        var request = new AddToDoItemRequest();
                        request.Items.AddRange(options.Select(x => x.ToAddToDoItemOptionsGrpc()).ToArray());

                        return client.AddToDoItemAsync(request, metadata, DateTime.UtcNow.Add(Timeout), ct)
                           .ToValueTaskResultValueOnly()
                           .ConfigureAwait(false)
                           .IfSuccessAsync(id => id.Ids.ToGuid().ToResult(), ct);
                    },
                    ct
                ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result<ToDoResponse>> GetAsync(GetToDo get, CancellationToken ct)
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(ct)
               .IfSuccessAsync(
                    metadata =>client.GetAsync(get.ToGetRequest(), metadata, DateTime.UtcNow.Add(Timeout), ct)
                       .ToValueTaskResultValueOnly()
                       .ConfigureAwait(false)
                       .IfSuccessAsync(reply => reply.ToToDoResponse().ToResult(), ct),
                    ct
                ),
            ct
        );
    }

    public Cvtar DeleteToDoItemsAsync(ReadOnlyMemory<Guid> ids, CancellationToken ct)
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(ct)
               .IfSuccessAsync(
                    metadata =>
                    {
                        var request = new DeleteToDoItemsRequest();
                        request.Ids.AddRange(ids.Select(x => x.ToByteString()).ToArray());

                        return client.DeleteToDoItemsAsync(request, metadata, DateTime.UtcNow.Add(Timeout), ct)
                           .ToValueTaskResultOnly()
                           .ConfigureAwait(false);
                    },
                    ct
                ),
            ct
        );
    }
}