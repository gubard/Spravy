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
    IGrpcServiceCreator<GrpcScheduleService, ScheduleServiceClient>
{
    private readonly IMapper mapper;
    private readonly IMetadataFactory metadataFactory;

    public GrpcScheduleService(
        IFactory<Uri, ScheduleServiceClient> grpcClientFactory,
        Uri host,
        IMapper mapper,
        IMetadataFactory metadataFactory
    ) : base(grpcClientFactory, host)
    {
        this.mapper = mapper;
        this.metadataFactory = metadataFactory;
    }

    public Task AddTimerAsync(AddTimerParameters parameters)
    {
        return CallClientAsync(
            async client =>
            {
                var request = mapper.Map<AddTimerRequest>(parameters);
                await client.AddTimerAsync(request, await metadataFactory.CreateAsync());
            }
        );
    }

    public Task<IEnumerable<TimerItem>> GetListTimesAsync()
    {
        return CallClientAsync(
            async client =>
            {
                var request = new GetListTimesRequest();
                var timers = await client.GetListTimesAsync(request, await metadataFactory.CreateAsync());

                return mapper.Map<IEnumerable<TimerItem>>(timers.Items);
            }
        );
    }

    public Task RemoveTimerAsync(Guid id)
    {
        return CallClientAsync(
            async client =>
            {
                await client.RemoveTimerAsync(
                    new RemoveTimerRequest
                    {
                        Id = mapper.Map<ByteString>(id),
                    },
                    await metadataFactory.CreateAsync()
                );
            }
        );
    }

    public static GrpcScheduleService CreateGrpcService(
        IFactory<Uri, ScheduleServiceClient> grpcClientFactory,
        Uri host,
        IMapper mapper,
        IMetadataFactory metadataFactory
    )
    {
        return new GrpcScheduleService(grpcClientFactory, host, mapper, metadataFactory);
    }
}