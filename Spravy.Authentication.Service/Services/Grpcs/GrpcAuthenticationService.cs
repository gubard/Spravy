using AutoMapper;
using Grpc.Core;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Authentication.Domain.Models;
using Spravy.Authentication.Protos;
using static Spravy.Authentication.Protos.AuthenticationService;

namespace Spravy.Authentication.Service.Services.Grpcs;

public class GrpcAuthenticationService : AuthenticationServiceBase
{
    private readonly IAuthenticationService authenticationService;
    private readonly IMapper mapper;

    public GrpcAuthenticationService(IAuthenticationService authenticationService, IMapper mapper)
    {
        this.authenticationService = authenticationService;
        this.mapper = mapper;
    }

    public override async Task<CreateUserReply> CreateUser(CreateUserRequest request, ServerCallContext context)
    {
        var options = mapper.Map<CreateUserOptions>(request);
        await authenticationService.CreateUserAsync(options);

        return new CreateUserReply();
    }

    public override async Task<IsValidReply> IsValid(IsValidRequest request, ServerCallContext context)
    {
        var user = mapper.Map<User>(request.User);
        var result = await authenticationService.IsValidAsync(user);

        return new IsValidReply
        {
            IsValid = result,
        };
    }
}