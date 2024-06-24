using System.Runtime.CompilerServices;
using Spravy.Client.Extensions;
using Spravy.Client.Interfaces;
using Spravy.Client.Services;
using Spravy.Core.Mappers;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.Schedule.Domain.Interfaces;
using Spravy.Schedule.Domain.Mapper.Mappers;
using Spravy.Schedule.Domain.Models;
using Spravy.Schedule.Protos;

namespace Spravy.Schedule.Domain.Client.Services;

public class GrpcScheduleService : GrpcServiceBase<ScheduleService.ScheduleServiceClient>,
    IScheduleService,
    IGrpcServiceCreatorAuth<GrpcScheduleService, ScheduleService.ScheduleServiceClient>
{
    private readonly IMetadataFactory metadataFactory;

    public GrpcScheduleService(
        IFactory<Uri, ScheduleService.ScheduleServiceClient> grpcClientFactory,
        Uri host,
        IMetadataFactory metadataFactory,
        ISerializer serializer
    ) : base(grpcClientFactory, host, serializer)
    {
        this.metadataFactory = metadataFactory;
    }

    public static GrpcScheduleService CreateGrpcService(
        IFactory<Uri, ScheduleService.ScheduleServiceClient> grpcClientFactory,
        Uri host,
        IMetadataFactory metadataFactory,
        ISerializer serializer
    )
    {
        return new(grpcClientFactory, host, metadataFactory, serializer);
    }

    public ConfiguredValueTaskAwaitable<Result> AddTimerAsync(
        AddTimerParameters parameters,
        CancellationToken ct
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(ct)
           .IfSuccessAsync(metadata => client.AddTimerAsync(new()
                {
                    Parameters = parameters.ToAddTimerParametersGrpc(),
                }, metadata, cancellationToken: ct)
               .ToValueTaskResultOnly()
               .ConfigureAwait(false), ct), ct);
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<TimerItem>>> GetListTimesAsync(
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(ct)
               .IfSuccessAsync(
                    value => client.GetListTimesAsync(new(), value)
                       .ToValueTaskResultValueOnly()
                       .ConfigureAwait(false)
                       .IfSuccessAsync(timers => timers.Items.ToTimerItem().ToResult(), ct),
                    ct), ct);
    }

    public ConfiguredValueTaskAwaitable<Result> RemoveTimerAsync(Guid id, CancellationToken ct)
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(ct)
           .IfSuccessAsync(metadata => client.RemoveTimerAsync(new()
                {
                    Id = id.ToByteString(),
                }, metadata)
               .ToValueTaskResultOnly()
               .ConfigureAwait(false), ct), ct);
    }
}