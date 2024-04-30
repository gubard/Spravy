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

    public override Task<UpdatePasswordByLoginReply> UpdatePasswordByLogin(
        UpdatePasswordByLoginRequest request,
        ServerCallContext context
    )
    {
        return authenticationService
           .UpdatePasswordByLoginAsync(request.Login, request.VerificationCode, request.NewPassword,
                context.CancellationToken)
           .HandleAsync<UpdatePasswordByLoginReply>(serializer);
    }

    public override Task<UpdatePasswordByEmailReply> UpdatePasswordByEmail(
        UpdatePasswordByEmailRequest request,
        ServerCallContext context
    )
    {
        return authenticationService
           .UpdatePasswordByEmailAsync(request.Email, request.VerificationCode, request.NewPassword,
                context.CancellationToken)
           .HandleAsync<UpdatePasswordByEmailReply>(serializer);
    }

    public override Task<UpdateEmailNotVerifiedUserByLoginReply> UpdateEmailNotVerifiedUserByLogin(
        UpdateEmailNotVerifiedUserByLoginRequest request,
        ServerCallContext context
    )
    {
        return authenticationService
           .UpdateEmailNotVerifiedUserByLoginAsync(request.Login, request.NewEmail, context.CancellationToken)
           .HandleAsync<UpdateEmailNotVerifiedUserByLoginReply>(serializer);
    }

    public override Task<UpdateEmailNotVerifiedUserByEmailReply> UpdateEmailNotVerifiedUserByEmail(
        UpdateEmailNotVerifiedUserByEmailRequest request,
        ServerCallContext context
    )
    {
        return authenticationService
           .UpdateEmailNotVerifiedUserByEmailAsync(request.Email, request.NewEmail, context.CancellationToken)
           .HandleAsync<UpdateEmailNotVerifiedUserByEmailReply>(serializer);
    }

    public override Task<UpdateVerificationCodeByLoginReply> UpdateVerificationCodeByLogin(
        UpdateVerificationCodeByLoginRequest request,
        ServerCallContext context
    )
    {
        return authenticationService.UpdateVerificationCodeByLoginAsync(request.Login, context.CancellationToken)
           .HandleAsync<UpdateVerificationCodeByLoginReply>(serializer);
    }

    public override Task<UpdateVerificationCodeByEmailReply> UpdateVerificationCodeByEmail(
        UpdateVerificationCodeByEmailRequest request,
        ServerCallContext context
    )
    {
        return authenticationService.UpdateVerificationCodeByEmailAsync(request.Email, context.CancellationToken)
           .HandleAsync<UpdateVerificationCodeByEmailReply>(serializer);
    }

    public override Task<VerifiedEmailByLoginReply> VerifiedEmailByLogin(
        VerifiedEmailByLoginRequest request,
        ServerCallContext context
    )
    {
        return authenticationService
           .VerifiedEmailByLoginAsync(request.Login, request.VerificationCode, context.CancellationToken)
           .HandleAsync<VerifiedEmailByLoginReply>(serializer);
    }

    public override Task<VerifiedEmailByEmailReply> VerifiedEmailByEmail(
        VerifiedEmailByEmailRequest request,
        ServerCallContext context
    )
    {
        return authenticationService
           .VerifiedEmailByEmailAsync(request.Email, request.VerificationCode, context.CancellationToken)
           .HandleAsync<VerifiedEmailByEmailReply>(serializer);
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

    public override Task<CreateUserReply> CreateUser(CreateUserRequest request, ServerCallContext context)
    {
        var options = mapper.Map<CreateUserOptions>(request);

        return authenticationService.CreateUserAsync(options, context.CancellationToken)
           .HandleAsync<CreateUserReply>(serializer);
    }

    public override Task<LoginReply> Login(LoginRequest request, ServerCallContext context)
    {
        var user = mapper.Map<User>(request.User);

        return authenticationService.LoginAsync(user, context.CancellationToken)
           .HandleAsync(serializer, value => mapper.Map<LoginReply>(value));
    }

    public override Task<RefreshTokenReply> RefreshToken(RefreshTokenRequest request, ServerCallContext context)
    {
        return authenticationService.RefreshTokenAsync(request.RefreshToken, context.CancellationToken)
           .HandleAsync(serializer, value => mapper.Map<RefreshTokenReply>(value));
    }
}