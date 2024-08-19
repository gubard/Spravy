namespace Spravy.EventBus.Domain.Client.Services;

public class EventBusServiceFactory : IFactory<string, IEventBusService>
{
    private readonly IFactory<
        Uri,
        EventBusService.EventBusServiceClient
    > eventBusServiceClientFactory;

    private readonly IMetadataFactory metadataFactory;
    private readonly GrpcEventBusServiceOptions options;
    private readonly IRpcExceptionHandler handler;
    private readonly IRetryService retryService;

    public EventBusServiceFactory(
        IFactory<Uri, EventBusService.EventBusServiceClient> eventBusServiceClientFactory,
        GrpcEventBusServiceOptions options,
        IRpcExceptionHandler handler,
        IRetryService retryService,
        IMetadataFactory metadataFactory
    )
    {
        this.eventBusServiceClientFactory = eventBusServiceClientFactory;
        this.options = options;
        this.handler = handler;
        this.retryService = retryService;
        this.metadataFactory = metadataFactory;
    }

    public Result<IEventBusService> Create(string key)
    {
        var host = options.Host.ThrowIfNull().ToUri();

        return new GrpcEventBusService(
            eventBusServiceClientFactory,
            host,
            metadataFactory,
            handler,
            retryService
        ).ToResult<IEventBusService>();
    }
}
