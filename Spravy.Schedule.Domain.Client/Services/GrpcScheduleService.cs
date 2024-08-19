namespace Spravy.Schedule.Domain.Client.Services;

public class GrpcScheduleService
    : GrpcServiceBase<ScheduleService.ScheduleServiceClient>,
        IScheduleService,
        IGrpcServiceCreatorAuth<GrpcScheduleService, ScheduleService.ScheduleServiceClient>
{
    private readonly IMetadataFactory metadataFactory;

    public GrpcScheduleService(
        IFactory<Uri, ScheduleService.ScheduleServiceClient> grpcClientFactory,
        Uri host,
        IMetadataFactory metadataFactory,
        IRpcExceptionHandler handler,
        IRetryService retryService
    )
        : base(grpcClientFactory, host, handler, retryService)
    {
        this.metadataFactory = metadataFactory;
    }

    public static GrpcScheduleService CreateGrpcService(
        IFactory<Uri, ScheduleService.ScheduleServiceClient> grpcClientFactory,
        Uri host,
        IMetadataFactory metadataFactory,
        IRpcExceptionHandler handler,
        IRetryService retryService
    )
    {
        return new(grpcClientFactory, host, metadataFactory, handler, retryService);
    }

    public ConfiguredValueTaskAwaitable<Result> AddTimerAsync(
        AddTimerParameters parameters,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .AddTimerAsync(
                                    parameters.ToAddTimerRequest(),
                                    metadata,
                                    cancellationToken: ct
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<TimerItem>>> GetTimersAsync(
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        value =>
                            client
                                .GetTimersAsync(new(), value)
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(
                                    timers => timers.Items.ToTimerItem().ToResult(),
                                    ct
                                ),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result> RemoveTimerAsync(Guid id, CancellationToken ct)
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .RemoveTimerAsync(new() { Id = id.ToByteString(), }, metadata)
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false),
                        ct
                    ),
            ct
        );
    }
}
