using AutoMapper;
using Google.Protobuf;
using Spravy.Client.Interfaces;
using Spravy.Client.Services;
using Spravy.Domain.Interfaces;
using Spravy.Schedule.Domain.Interfaces;
using Spravy.Schedule.Domain.Models;
using Spravy.Schedule.Protos;
using static Spravy.Schedule.Protos.ScheduleService;

namespace Spravy.Schedule.Domain.Client.Services;

public class GrpcScheduleService : GrpcServiceBase<ScheduleServiceClient>,
    IScheduleService,
    IGrpcServiceCreatorAuth<GrpcScheduleService, ScheduleServiceClient>
{
    private readonly IMapper mapper;
    private readonly IMetadataFactory metadataFactory;

    public GrpcScheduleService(
        IFactory<Uri, ScheduleServiceClient> grpcClientFactory,
        Uri host,
        IMapper mapper,
        IMetadataFactory metadataFactory,
        ISerializer serializer
    ) : base(grpcClientFactory, host, serializer)
    {
        this.mapper = mapper;
        this.metadataFactory = metadataFactory;
    }

    public Task AddTimerAsync(AddTimerParameters parameters, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);
                var request = mapper.Map<AddTimerRequest>(parameters);
                cancellationToken.ThrowIfCancellationRequested();
                await client.AddTimerAsync(request, metadata);
            },
            cancellationToken
        );
    }

    public Task<IEnumerable<TimerItem>> GetListTimesAsync(CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);
                var request = new GetListTimesRequest();
                cancellationToken.ThrowIfCancellationRequested();
                var timers = await client.GetListTimesAsync(request, metadata);

                return mapper.Map<IEnumerable<TimerItem>>(timers.Items);
            },
            cancellationToken
        );
    }

    public Task RemoveTimerAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                await client.RemoveTimerAsync(
                    new RemoveTimerRequest
                    {
                        Id = mapper.Map<ByteString>(id),
                    },
                    metadata
                );
            },
            cancellationToken
        );
    }

    public static GrpcScheduleService CreateGrpcService(
        IFactory<Uri, ScheduleServiceClient> grpcClientFactory,
        Uri host,
        IMapper mapper,
        IMetadataFactory metadataFactory,
        ISerializer serializer
    )
    {
        return new GrpcScheduleService(grpcClientFactory, host, mapper, metadataFactory, serializer);
    }
}