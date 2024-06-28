using Spravy.Client.Services;
using Spravy.Core.Interfaces;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.Domain.Services;
using Spravy.EventBus.Domain.Client.Models;
using Spravy.EventBus.Domain.Interfaces;
using Spravy.EventBus.Protos;

namespace Spravy.EventBus.Domain.Client.Services;

public class EventBusServiceFactory : IFactory<string, IEventBusService>
{
    private readonly IFactory<
        Uri,
        EventBusService.EventBusServiceClient
    > eventBusServiceClientFactory;
    private readonly IHttpHeaderFactory httpHeaderFactory;
    private readonly GrpcEventBusServiceOptions options;
    private readonly IRpcExceptionHandler handler;
    private readonly ITokenService tokenService;

    public EventBusServiceFactory(
        IHttpHeaderFactory httpHeaderFactory,
        IFactory<Uri, EventBusService.EventBusServiceClient> eventBusServiceClientFactory,
        GrpcEventBusServiceOptions options,
        ITokenService tokenService,
        IRpcExceptionHandler handler
    )
    {
        this.httpHeaderFactory = httpHeaderFactory;
        this.eventBusServiceClientFactory = eventBusServiceClientFactory;
        this.options = options;
        this.tokenService = tokenService;
        this.handler = handler;
    }

    public Result<IEventBusService> Create(string key)
    {
        if (!options.Token.IsNullOrWhiteSpace())
        {
            tokenService.LoginAsync(options.Token, CancellationToken.None).GetAwaiter().GetResult();
        }

        var headers = new[]
        {
            httpHeaderFactory,
            new ValuesHttpHeaderFactory(HttpHeaderItem.CreateUserId(key).ToReadOnlyMemory()),
            new TokenHttpHeaderFactory(tokenService),
        };

        var metadataFactory = new MetadataFactory(new CombineHttpHeaderFactory(headers));
        var host = options.Host.ThrowIfNull().ToUri();

        return new GrpcEventBusService(
            eventBusServiceClientFactory,
            host,
            metadataFactory,
            handler
        ).ToResult<IEventBusService>();
    }
}
