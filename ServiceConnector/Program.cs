using Grpc.Core;
using Spravy.Authentication.Service.Services;
using Spravy.Client.Enums;
using Spravy.Client.Services;
using Spravy.Core.Services;
using Spravy.Domain.Extensions;
using Spravy.Domain.Models;
using Spravy.Domain.Services;
using Spravy.EventBus.Domain.Client.Services;
using Spravy.EventBus.Protos;
using Spravy.ToDo.Domain.Models;
using RpcExceptionHandler = Spravy.Ui.Services.RpcExceptionHandler;
using SpravyJsonSerializerContext = Spravy.Ui.Services.SpravyJsonSerializerContext;

var jwtTokenFactory = new JwtTokenFactory(
    new()
    {
        Audience = "https://spravy.audience.authentication.com",
        Issuer = "https://spravy.issuer.authentication.com",
        Key = "0bf7731f-2441-4cff-8e2e-7b343d5d35d0b9b47d13-5b69-4249-aed9-24421e8a94d9",
        ExpiresDays = 1,
        RefreshExpiresDays = 7,
    },
    new()
);

var token = jwtTokenFactory.Create();

var eventBusService = new GrpcEventBusService(
    new GrpcClientFactory<EventBusService.EventBusServiceClient>(
        new GrpcChannelFactory(GrpcChannelType.Default, ChannelCredentials.Insecure),
        new EventBusServiceClientFactory()
    ),
    new("http://localhost:5001"),
    new MetadataFactory(
        new ValuesHttpHeaderFactory(
            new(
                [
                    HttpHeaderItem.CreateBearerAuthorization(token.ThrowIfError().Token),
                    HttpHeaderItem.CreateUserId("32553555-290F-47D4-BEA0-B99A002CF96D"),
                ]
            )
        )
    ),
    new RpcExceptionHandler(new SpravyJsonSerializer(new SpravyJsonSerializerContext())),
    new RetryService()
);

var events = await eventBusService.GetEventsAsync(
    new([AddToDoItemToFavoriteEvent.EventId,]),
    CancellationToken.None
);

if (events.TryGetValue(out var result))
{
    foreach (var error in result.ToArray())
    {
        Console.WriteLine(error.Id);
    }
}
else
{
    foreach (var error in events.Errors.ToArray())
    {
        Console.WriteLine(error.Message);
    }
}
