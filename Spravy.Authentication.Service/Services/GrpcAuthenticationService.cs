using AutoMapper;
using Grpc.Core;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Authentication.Domain.Models;
using Spravy.Authentication.Protos;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.Service.Extensions;
using static Spravy.Authentication.Protos.AuthenticationService;

namespace Spravy.Authentication.Service.Services;

public class GrpcAuthenticationService : AuthenticationServiceBase
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

    public override async Task<UpdatePasswordByLoginReply> UpdatePasswordByLogin(
        UpdatePasswordByLoginRequest request,
        ServerCallContext context
    )
    {
        await authenticationService.UpdatePasswordByLoginAsync(
            request.Login,
            request.VerificationCode,
            request.NewPassword,
            context.CancellationToken
        );

        return new UpdatePasswordByLoginReply();
    }

    public override async Task<UpdatePasswordByEmailReply> UpdatePasswordByEmail(
        UpdatePasswordByEmailRequest request,
        ServerCallContext context
    )
    {
        await authenticationService.UpdatePasswordByEmailAsync(
            request.Email,
            request.VerificationCode,
            request.NewPassword,
            context.CancellationToken
        );

        return new UpdatePasswordByEmailReply();
    }

    public override async Task<UpdateEmailNotVerifiedUserByLoginReply> UpdateEmailNotVerifiedUserByLogin(
        UpdateEmailNotVerifiedUserByLoginRequest request,
        ServerCallContext context
    )
    {
        await authenticationService.UpdateEmailNotVerifiedUserByLoginAsync(
            request.Login,
            request.NewEmail,
            context.CancellationToken
        );

        return new UpdateEmailNotVerifiedUserByLoginReply();
    }

    public override async Task<UpdateEmailNotVerifiedUserByEmailReply> UpdateEmailNotVerifiedUserByEmail(
        UpdateEmailNotVerifiedUserByEmailRequest request,
        ServerCallContext context
    )
    {
        await authenticationService.UpdateEmailNotVerifiedUserByEmailAsync(
            request.Email,
            request.NewEmail,
            context.CancellationToken
        );

        return new UpdateEmailNotVerifiedUserByEmailReply();
    }

    public override async Task<UpdateVerificationCodeByLoginReply> UpdateVerificationCodeByLogin(
        UpdateVerificationCodeByLoginRequest request,
        ServerCallContext context
    )
    {
        await authenticationService.UpdateVerificationCodeByLoginAsync(
            request.Login,
            context.CancellationToken
        );

        return new UpdateVerificationCodeByLoginReply();
    }

    public override async Task<UpdateVerificationCodeByEmailReply> UpdateVerificationCodeByEmail(
        UpdateVerificationCodeByEmailRequest request,
        ServerCallContext context
    )
    {
        await authenticationService.UpdateVerificationCodeByEmailAsync(
            request.Email,
            context.CancellationToken
        );

        return new UpdateVerificationCodeByEmailReply();
    }

    public override async Task<VerifiedEmailByLoginReply> VerifiedEmailByLogin(
        VerifiedEmailByLoginRequest request,
        ServerCallContext context
    )
    {
        await authenticationService.VerifiedEmailByLoginAsync(
            request.Login,
            request.VerificationCode,
            context.CancellationToken
        );

        return new VerifiedEmailByLoginReply();
    }

    public override async Task<VerifiedEmailByEmailReply> VerifiedEmailByEmail(
        VerifiedEmailByEmailRequest request,
        ServerCallContext context
    )
    {
        await authenticationService.VerifiedEmailByEmailAsync(
            request.Email,
            request.VerificationCode,
            context.CancellationToken
        );

        return new VerifiedEmailByEmailReply();
    }

    public override async Task<IsVerifiedByLoginReply> IsVerifiedByLogin(
        IsVerifiedByLoginRequest request,
        ServerCallContext context
    )
    {
        var result = await authenticationService.IsVerifiedByLoginAsync(request.Login, context.CancellationToken);

        return new IsVerifiedByLoginReply
        {
            IsVerified = result
        };
    }

    public override async Task<IsVerifiedByEmailReply> IsVerifiedByEmail(
        IsVerifiedByEmailRequest request,
        ServerCallContext context
    )
    {
        var result = await authenticationService.IsVerifiedByEmailAsync(request.Email, context.CancellationToken);

        return new IsVerifiedByEmailReply
        {
            IsVerified = result
        };
    }

    public override async Task<CreateUserReply> CreateUser(CreateUserRequest request, ServerCallContext context)
    {
        var options = mapper.Map<CreateUserOptions>(request);
        var error = await authenticationService.CreateUserAsync(options, context.CancellationToken);

        if (!error.IsError)
        {
            return new CreateUserReply();
        }

        throw await error.ToRpcExceptionAsync(serializer);
    }

    public override async Task<LoginReply> Login(LoginRequest request, ServerCallContext context)
    {
        var user = mapper.Map<User>(request.User);
        var result = await authenticationService.LoginAsync(user, context.CancellationToken);
        var reply = mapper.Map<LoginReply>(result);

        return reply;
    }

    public override async Task<RefreshTokenReply> RefreshToken(RefreshTokenRequest request, ServerCallContext context)
    {
        var result = await authenticationService.RefreshTokenAsync(request.RefreshToken, context.CancellationToken);
        var reply = mapper.Map<RefreshTokenReply>(result);

        return reply;
    }
}