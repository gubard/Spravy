using AutoMapper;
using Google.Protobuf;
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

    public Task<Result> AddTimerAsync(AddTimerParameters parameters, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            var request = mapper.Map<AddTimerRequest>(parameters);
                            cancellationToken.ThrowIfCancellationRequested();
                            await client.AddTimerAsync(request, value);

                            return Result.Success;
                        }
                    );
            },
            cancellationToken
        );
    }

    public Task<Result<ReadOnlyMemory<TimerItem>>> GetListTimesAsync(CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            var request = new GetListTimesRequest();
                            cancellationToken.ThrowIfCancellationRequested();
                            var timers = await client.GetListTimesAsync(request, value);

                            return mapper.Map<ReadOnlyMemory<TimerItem>>(timers.Items).ToResult();
                        }
                    );
            },
            cancellationToken
        );
    }

    public Task<Result> RemoveTimerAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            await client.RemoveTimerAsync(
                                new RemoveTimerRequest
                                {
                                    Id = mapper.Map<ByteString>(id),
                                },
                                value
                            );

                            return Result.Success;
                        }
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