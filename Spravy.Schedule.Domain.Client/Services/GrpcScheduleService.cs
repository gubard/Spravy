using Google.Protobuf;
using Spravy.Client.Extensions;
using Spravy.Client.Interfaces;
using Spravy.Client.Services;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.Schedule.Domain.Interfaces;
using Spravy.Schedule.Domain.Models;
using Spravy.Schedule.Protos;
using static Spravy.Schedule.Protos.ScheduleService;

namespace Spravy.Schedule.Domain.Client.Services;

public class GrpcScheduleService : GrpcServiceBase<ScheduleServiceClient>,
    IScheduleService,
    IGrpcServiceCreatorAuth<GrpcScheduleService, ScheduleServiceClient>
{
    private readonly IConverter converter;
    private readonly IMetadataFactory metadataFactory;

    public GrpcScheduleService(
        IFactory<Uri, ScheduleServiceClient> grpcClientFactory,
        Uri host,
        IConverter converter,
        IMetadataFactory metadataFactory,
        ISerializer serializer
    ) : base(grpcClientFactory, host, serializer)
    {
        this.converter = converter;
        this.metadataFactory = metadataFactory;
    }

    public ValueTask<Result> AddTimerAsync(AddTimerParameters parameters, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .ConfigureAwait(false)
                    .IfSuccessAsync(
                        converter.Convert<AddTimerRequest>(parameters),
                        (value, request) => client.AddTimerAsync(request, value)
                            .ToValueTaskResultOnly()
                            .ConfigureAwait(false)
                    )
                    .ConfigureAwait(false),
            cancellationToken
        );
    }

    public ValueTask<Result<ReadOnlyMemory<TimerItem>>> GetListTimesAsync(CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .ConfigureAwait(false)
                    .IfSuccessAsync(
                        value =>
                            client.GetListTimesAsync(new GetListTimesRequest(), value)
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(
                                    timers => converter.Convert<TimerItem[]>(timers.Items)
                                        .IfSuccess(items => items.ToReadOnlyMemory().ToResult())
                                        .ToValueTaskResult()
                                        .ConfigureAwait(false)
                                )
                                .ConfigureAwait(false)
                    )
                    .ConfigureAwait(false),
            cancellationToken
        );
    }

    public ValueTask<Result> RemoveTimerAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .ConfigureAwait(false)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        (value, i) =>
                            client.RemoveTimerAsync(
                                    new RemoveTimerRequest
                                    {
                                        Id = i,
                                    },
                                    value
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false)
                    )
                    .ConfigureAwait(false),
            cancellationToken
        );
    }

    public static GrpcScheduleService CreateGrpcService(
        IFactory<Uri, ScheduleServiceClient> grpcClientFactory,
        Uri host,
        IConverter converter,
        IMetadataFactory metadataFactory,
        ISerializer serializer
    )
    {
        return new GrpcScheduleService(grpcClientFactory, host, converter, metadataFactory, serializer);
    }
}