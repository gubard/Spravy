using AutoMapper;
using Grpc.Core;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Authentication.Domain.Models;
using Spravy.Authentication.Protos;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.Service.Extensions;

namespace Spravy.Router.Service.Services;

public class GrpcAuthenticationService : AuthenticationService.AuthenticationServiceBase
{
    private readonly IAuthenticationService authenticationService;
    private readonly IMapper mapper;
    private readonly ISerializer serializer;

    public GrpcAuthenticationService(
        IAuthenticationService authenticationService,
        IMapper mapper,
        ISerializer serializer
    )
    {
        this.authenticationService = authenticationService;
        this.mapper = mapper;
        this.serializer = serializer;
    }

    public override async Task<UpdateEmailNotVerifiedUserByLoginReply> UpdateEmailNotVerifiedUserByLogin(
        UpdateEmailNotVerifiedUserByLoginRequest request,
        ServerCallContext context
    )
    {
        await authenticationService.UpdateEmailNotVerifiedUserByLoginAsync(request.Login, request.NewEmail,
            context.CancellationToken);

        return new();
    }

    public override async Task<UpdateEmailNotVerifiedUserByEmailReply> UpdateEmailNotVerifiedUserByEmail(
        UpdateEmailNotVerifiedUserByEmailRequest request,
        ServerCallContext context
    )
    {
        await authenticationService.UpdateEmailNotVerifiedUserByEmailAsync(request.Email, request.NewEmail,
            context.CancellationToken);

        return new();
    }

    public override async Task<UpdateVerificationCodeByLoginReply> UpdateVerificationCodeByLogin(
        UpdateVerificationCodeByLoginRequest request,
        ServerCallContext context
    )
    {
        await authenticationService.UpdateVerificationCodeByLoginAsync(request.Login, context.CancellationToken);

        return new();
    }

    public override async Task<UpdateVerificationCodeByEmailReply> UpdateVerificationCodeByEmail(
        UpdateVerificationCodeByEmailRequest request,
        ServerCallContext context
    )
    {
        await authenticationService.UpdateVerificationCodeByEmailAsync(request.Email, context.CancellationToken);

        return new();
    }

    public override async Task<VerifiedEmailByLoginReply> VerifiedEmailByLogin(
        VerifiedEmailByLoginRequest request,
        ServerCallContext context
    )
    {
        await authenticationService.VerifiedEmailByLoginAsync(request.Login, request.VerificationCode,
            context.CancellationToken);

        return new();
    }

    public override async Task<VerifiedEmailByEmailReply> VerifiedEmailByEmail(
        VerifiedEmailByEmailRequest request,
        ServerCallContext context
    )
    {
        await authenticationService.VerifiedEmailByEmailAsync(request.Email, request.VerificationCode,
            context.CancellationToken);

        return new();
    }

    public override async Task<UpdatePasswordByLoginReply> UpdatePasswordByLogin(
        UpdatePasswordByLoginRequest request,
        ServerCallContext context
    )
    {
        await authenticationService.UpdatePasswordByLoginAsync(request.Login, request.VerificationCode,
            request.NewPassword, context.CancellationToken);

        return new();
    }

    public override async Task<UpdatePasswordByEmailReply> UpdatePasswordByEmail(
        UpdatePasswordByEmailRequest request,
        ServerCallContext context
    )
    {
        await authenticationService.UpdatePasswordByEmailAsync(request.Email, request.VerificationCode,
            request.NewPassword, context.CancellationToken);

        return new();
    }

    public override Task<IsVerifiedByLoginReply> IsVerifiedByLogin(
        IsVerifiedByLoginRequest request,
        ServerCallContext context
    )
    {
        return authenticationService.IsVerifiedByLoginAsync(request.Login, context.CancellationToken)
           .HandleAsync(serializer, value => new IsVerifiedByLoginReply
            {
                IsVerified = value,
            });
    }

    public override Task<IsVerifiedByEmailReply> IsVerifiedByEmail(
        IsVerifiedByEmailRequest request,
        ServerCallContext context
    )
    {
        return authenticationService.IsVerifiedByEmailAsync(request.Email, context.CancellationToken)
           .HandleAsync(serializer, value => new IsVerifiedByEmailReply
            {
                IsVerified = value,
            });
    }

    public override async Task<CreateUserReply> CreateUser(CreateUserRequest request, ServerCallContext context)
    {
        var options = mapper.Map<CreateUserOptions>(request);
        await authenticationService.CreateUserAsync(options, context.CancellationToken);

        return new();
    }

    public override Task<LoginReply> Login(LoginRequest request, ServerCallContext context)
    {
        var user = mapper.Map<User>(request.User);

        return authenticationService.LoginAsync(user, context.CancellationToken)
           .HandleAsync(serializer, token => mapper.Map<LoginReply>(token));
    }

    public override async Task<RefreshTokenReply> RefreshToken(RefreshTokenRequest request, ServerCallContext context)
    {
        var result = await authenticationService.RefreshTokenAsync(request.RefreshToken, context.CancellationToken);
        var reply = mapper.Map<RefreshTokenReply>(result);

        return reply;
    }
}