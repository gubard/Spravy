using AutoMapper;
using Google.Protobuf;
using Grpc.Core;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Authentication.Domain.Models;
using Spravy.Authentication.Protos;
using Spravy.EventBus.Domain.Helpers;
using Spravy.EventBus.Domain.Interfaces;
using Spravy.EventBus.Protos;
using static Spravy.Authentication.Protos.AuthenticationService;

namespace Spravy.Authentication.Service.Services;

public class GrpcAuthenticationService : AuthenticationServiceBase
{
    private readonly IAuthenticationService authenticationService;
    private readonly IMapper mapper;
    private readonly IEventBusService eventBusService;

    public GrpcAuthenticationService(
        IAuthenticationService authenticationService,
        IMapper mapper,
        IEventBusService eventBusService
    )
    {
        this.authenticationService = authenticationService;
        this.mapper = mapper;
        this.eventBusService = eventBusService;
    }

    public override async Task<CreateUserReply> CreateUser(CreateUserRequest request, ServerCallContext context)
    {
        var options = mapper.Map<CreateUserOptions>(request);
        var token = await authenticationService.CreateUserAsync(options);

        var eventContent = new CreateUserEvent
        {
            Token = token.Token
        };

        await using var stream = new MemoryStream();
        eventContent.WriteTo(stream);
        stream.Position = 0;
        await eventBusService.PublishEventAsync(EventIdHelper.CreateUserId, stream);

        return new CreateUserReply
        {
            Token = token.Token
        };
    }

    public override async Task<LoginReply> Login(LoginRequest request, ServerCallContext context)
    {
        var user = mapper.Map<User>(request.User);
        var result = await authenticationService.LoginAsync(user);
        var reply = mapper.Map<LoginReply>(result);

        return reply;
    }
}