using AutoMapper;
using Google.Protobuf;
using Grpc.Core;
using Spravy.Authentication.Domain.Models;
using Spravy.Client.Exceptions;
using Spravy.Client.Extensions;
using Spravy.Client.Services;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
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
    private readonly IKeeper<TokenResult> tokenKeeper;

    public GrpcScheduleService(GrpcScheduleServiceOptions options, IMapper mapper, IKeeper<TokenResult> tokenKeeper)
        : base(options.Host.ToUri(), options.ChannelType, options.ChannelCredentialType.GetChannelCredentials())
    {
        this.mapper = mapper;
        this.tokenKeeper = tokenKeeper;
        client = new ScheduleServiceClient(grpcChannel);
    }

    public async Task AddTimerAsync(AddTimerParameters parameters)
    {
        try
        {
            var request = mapper.Map<AddTimerRequest>(parameters);
            await client.AddTimerAsync(request, CreateMetadata());
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task<IEnumerable<TimerItem>> GetListTimesAsync()
    {
        try
        {
            var timers = await client.GetListTimesAsync(new GetListTimesRequest(), CreateMetadata());

            return mapper.Map<IEnumerable<TimerItem>>(timers.Items);
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
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
                CreateMetadata()
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    private Metadata CreateMetadata()
    {
        var metadata = new Metadata
        {
            {
                "Authorization", $"Bearer {tokenKeeper.Get().Token}"
            }
        };

        return metadata;
    }
}