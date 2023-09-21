using AutoMapper;
using Google.Protobuf;
using Spravy.Client.Exceptions;
using Spravy.Client.Extensions;
using Spravy.Client.Interfaces;
using Spravy.Client.Services;
using Spravy.Domain.Extensions;
using Spravy.Schedule.Domain.Client.Models;
using Spravy.Schedule.Domain.Interfaces;
using Spravy.Schedule.Domain.Models;
using Spravy.Schedule.Protos;
using static Spravy.Schedule.Protos.ScheduleService;

namespace Spravy.Schedule.Domain.Client.Services;

public class GrpcScheduleService : GrpcServiceBase, IScheduleService
{
    private readonly ScheduleServiceClient client;
    private readonly IMapper mapper;
    private readonly IMetadataFactory metadataFactory;

    public GrpcScheduleService(
        GrpcScheduleServiceOptions options,
        IMapper mapper,
        IMetadataFactory metadataFactory
    )
        : base(
            options.Host.ThrowIfNullOrWhiteSpace().ToUri(),
            options.ChannelType,
            options.ChannelCredentialType.GetChannelCredentials()
        )
    {
        this.mapper = mapper;
        this.metadataFactory = metadataFactory;
        client = new ScheduleServiceClient(GrpcChannel);
    }

    public async Task AddTimerAsync(AddTimerParameters parameters)
    {
        try
        {
            var request = mapper.Map<AddTimerRequest>(parameters);
            await client.AddTimerAsync(request, await metadataFactory.CreateAsync());
        }
        catch (Exception e)
        {
            throw new GrpcException(GrpcChannel.Target, e);
        }
    }

    public async Task<IEnumerable<TimerItem>> GetListTimesAsync()
    {
        try
        {
            var timers = await client.GetListTimesAsync(new GetListTimesRequest(), await metadataFactory.CreateAsync());

            return mapper.Map<IEnumerable<TimerItem>>(timers.Items);
        }
        catch (Exception e)
        {
            throw new GrpcException(GrpcChannel.Target, e);
        }
    }

    public async Task RemoveTimerAsync(Guid id)
    {
        try
        {
            await client.RemoveTimerAsync(
                new RemoveTimerRequest
                {
                    Id = mapper.Map<ByteString>(id),
                },
                await metadataFactory.CreateAsync()
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(GrpcChannel.Target, e);
        }
    }
}