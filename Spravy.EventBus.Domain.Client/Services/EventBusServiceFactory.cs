using AutoMapper;
using Spravy.Client.Services;
using Spravy.Domain.Extensions;
using Spravy.Domain.Helpers;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.Domain.Services;
using Spravy.EventBus.Domain.Client.Models;
using Spravy.EventBus.Domain.Interfaces;

namespace Spravy.EventBus.Domain.Client.Services;

public class EventBusServiceFactory : IFactory<string, IEventBusService>
{
    private readonly GrpcEventBusServiceOptions options;
    private readonly IMapper mapper;
    private readonly IHttpHeaderFactory httpHeaderFactory;

    public EventBusServiceFactory(
        GrpcEventBusServiceOptions options,
        IMapper mapper,
        IHttpHeaderFactory httpHeaderFactory
    )
    {
        this.options = options;
        this.mapper = mapper;
        this.httpHeaderFactory = httpHeaderFactory;
    }

    public IEventBusService Create(string key)
    {
        var headers = new[]
        {
            httpHeaderFactory, new ValuesHttpHeaderFactory(HttpHeaderItem.CreateUserId(key).ToEnumerable()),
        };

        return new GrpcEventBusService(options, mapper, new MetadataFactory(new CombineHttpHeaderFactory(headers)));
    }
}