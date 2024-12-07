namespace Spravy.PasswordGenerator.Service.Services;

[Authorize]
public class GrpcPasswordService : PasswordServiceBase
{
    private readonly IPasswordService passwordService;
    private readonly ISerializer serializer;

    public GrpcPasswordService(IPasswordService passwordService, ISerializer serializer)
    {
        this.passwordService = passwordService;
        this.serializer = serializer;
    }

    public override Task<EditPasswordItemsReply> EditPasswordItems(
        EditPasswordItemsRequest request,
        ServerCallContext context
    )
    {
        return passwordService.EditPasswordItemsAsync(request.Value.ToEditPasswordItems(), context.CancellationToken)
           .HandleAsync<EditPasswordItemsReply>(serializer, context.CancellationToken);
    }

    public override Task<GeneratePasswordReply> GeneratePassword(
        GeneratePasswordRequest request,
        ServerCallContext context
    )
    {
        return passwordService.GeneratePasswordAsync(request.Id.ToGuid(), context.CancellationToken)
           .HandleAsync(
                serializer,
                value => new GeneratePasswordReply
                {
                    Password = value,
                },
                context.CancellationToken
            );
    }

    public override Task<GetPasswordItemReply> GetPasswordItem(
        GetPasswordItemRequest request,
        ServerCallContext context
    )
    {
        return passwordService.GetPasswordItemAsync(request.Id.ToGuid(), context.CancellationToken)
           .HandleAsync(serializer, value => value.ToGetPasswordItemReply(), context.CancellationToken);
    }

    public override Task<AddPasswordItemReply> AddPasswordItem(
        AddPasswordItemRequest request,
        ServerCallContext context
    )
    {
        return passwordService.AddPasswordItemAsync(request.ToAddPasswordOptions(), context.CancellationToken)
           .HandleAsync<AddPasswordItemReply>(serializer, context.CancellationToken);
    }

    public override Task<GetPasswordItemsReply> GetPasswordItems(
        GetPasswordItemsRequest request,
        ServerCallContext context
    )
    {
        return passwordService.GetPasswordItemsAsync(context.CancellationToken)
           .HandleAsync(
                serializer,
                value =>
                {
                    var reply = new GetPasswordItemsReply();
                    reply.Items.AddRange(value.ToPasswordItemGrpc().ToArray());

                    return reply;
                },
                context.CancellationToken
            );
    }

    public override Task<DeletePasswordItemReply> DeletePasswordItem(
        DeletePasswordItemRequest request,
        ServerCallContext context
    )
    {
        return passwordService.DeletePasswordItemAsync(request.Id.ToGuid(), context.CancellationToken)
           .HandleAsync<DeletePasswordItemReply>(serializer, context.CancellationToken);
    }
}