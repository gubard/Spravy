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

    public Task<Result> AddTimerAsync(AddTimerParameters parameters, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<AddTimerRequest>(parameters),
                        async (value, request) =>
                        {
                            await client.AddTimerAsync(request, value);

                            return Result.Success;
                        }
                    ),
            cancellationToken
        );
    }

    public Task<Result<ReadOnlyMemory<TimerItem>>> GetListTimesAsync(CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            var request = new GetListTimesRequest();
                            var timers = await client.GetListTimesAsync(request, value);

                            return converter.Convert<TimerItem[]>(timers.Items)
                                .IfSuccess(items => items.ToReadOnlyMemory().ToResult());
                        }
                    ),
            cancellationToken
        );
    }

    public Task<Result> RemoveTimerAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        async (value, i) =>
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            await client.RemoveTimerAsync(
                                new RemoveTimerRequest
                                {
                                    Id = i,
                                },
                                value
                            );

                            return Result.Success;
                        }
                    ),
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