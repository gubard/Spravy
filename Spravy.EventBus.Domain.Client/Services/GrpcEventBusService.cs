using System.Runtime.CompilerServices;
using Google.Protobuf;
using Spravy.Client.Extensions;
using Spravy.Client.Interfaces;
using Spravy.Client.Services;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.EventBus.Domain.Interfaces;
using Spravy.EventBus.Domain.Models;
using Spravy.EventBus.Protos;
using static Spravy.EventBus.Protos.EventBusService;

namespace Spravy.EventBus.Domain.Client.Services;

public class GrpcEventBusService : GrpcServiceBase<EventBusServiceClient>,
    IEventBusService,
    IGrpcServiceCreatorAuth<GrpcEventBusService, EventBusServiceClient>
{
    private readonly IConverter converter;
    private readonly IMetadataFactory metadataFactory;

    public GrpcEventBusService(
        IFactory<Uri, EventBusServiceClient> grpcClientFactory,
        Uri host,
        IConverter converter,
        IMetadataFactory metadataFactory,
        ISerializer serializer
    ) : base(grpcClientFactory, host, serializer)
    {
        this.converter = converter;
        this.metadataFactory = metadataFactory;
    }

    public IAsyncEnumerable<EventValue> SubscribeEventsAsync(Guid[] eventIds, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            (client, token) => SubscribeEventsAsyncCore(client, eventIds, token),
            cancellationToken
        );
    }

    public ValueTask<Result> PublishEventAsync(Guid eventId, byte[] content, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(cancellationToken)
                .ConfigureAwait(false)
                .IfSuccessAsync(
                    converter.Convert<ByteString>(eventId),
                    converter.Convert<ByteString>(content),
                    (value, ei, c) =>
                    {
                        var request = new PublishEventRequest
                        {
                            EventId = ei,
                            Content = c,
                        };

                        return client.PublishEventAsync(request, value, cancellationToken: cancellationToken)
                            .ToValueTaskResultOnly()
                            .ConfigureAwait(false);
                    }
                )
                .ConfigureAwait(false),
            cancellationToken
        );
    }

    public ValueTask<Result<ReadOnlyMemory<EventValue>>> GetEventsAsync(
        ReadOnlyMemory<Guid> eventIds,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .ConfigureAwait(false)
                    .IfSuccessAsync(
                        converter.Convert<ByteString[]>(eventIds.ToArray()),
                        (value, ei) =>
                        {
                            var request = new GetEventsRequest();
                            request.EventIds.AddRange(ei);

                            return client.GetEventsAsync(
                                    request,
                                    value,
                                    cancellationToken: cancellationToken
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(
                                    events => converter.Convert<EventValue[]>(events.Events)
                                        .IfSuccess(
                                            e => e.ToReadOnlyMemory()
                                                .ToResult()
                                        )
                                        .ToValueTaskResult()
                                        .ConfigureAwait(false)
                                )
                                .ConfigureAwait(false);
                        }
                    )
                    .ConfigureAwait(false),
            cancellationToken
        );
    }

    private async IAsyncEnumerable<EventValue> SubscribeEventsAsyncCore(
        EventBusServiceClient client,
        Guid[] eventIds,
        [EnumeratorCancellation] CancellationToken cancellationToken
    )
    {
        var request = new SubscribeEventsRequest();
        request.EventIds.AddRange(converter.Convert<ByteString[]>(eventIds).Value);
        cancellationToken.ThrowIfCancellationRequested();
        var metadata = await metadataFactory.CreateAsync(cancellationToken);
        using var response = client.SubscribeEvents(request, metadata.Value, cancellationToken: cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();

        while (await response.ResponseStream.MoveNext(cancellationToken))
        {
            var reply = response.ResponseStream.Current;
            var eventValue = converter.Convert<EventValue>(reply);

            yield return eventValue.Value;

            cancellationToken.ThrowIfCancellationRequested();
        }
    }

    public static GrpcEventBusService CreateGrpcService(
        IFactory<Uri, EventBusServiceClient> grpcClientFactory,
        Uri host,
        IConverter converter,
        IMetadataFactory metadataFactory,
        ISerializer serializer
    )
    {
        return new(grpcClientFactory, host, converter, metadataFactory, serializer);
    }
}