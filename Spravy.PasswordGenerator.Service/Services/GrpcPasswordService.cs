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

    public override Task<AddPasswordItemReply> AddPasswordItem(
        AddPasswordItemRequest request,
        ServerCallContext context
    )
    {
        return passwordService.AddPasswordItemAsync(request.ToAddPasswordOptions(), context.CancellationToken)
           .HandleAsync<AddPasswordItemReply>(serializer, context.CancellationToken);
    }

    public override Task<DeletePasswordItemReply> DeletePasswordItem(
        DeletePasswordItemRequest request,
        ServerCallContext context
    )
    {
        return passwordService.DeletePasswordItemAsync(request.Id.ToGuid(), context.CancellationToken)
           .HandleAsync<DeletePasswordItemReply>(serializer, context.CancellationToken);
    }

    public override async Task GetPasswordItems(
        GetPasswordItemsRequest request,
        IServerStreamWriter<GetPasswordItemsReply> responseStream,
        ServerCallContext context
    )
    {
        var ids = request.Ids.ToGuid();

        await foreach (var item in passwordService.GetPasswordItemsAsync(
                ids,
                request.ChunkSize,
                context.CancellationToken
            ))
        {
            var reply = new GetPasswordItemsReply();
            reply.Items.AddRange(item.ThrowIfNull().ThrowIfError().ToPasswordItemGrpc().ToArray());
            await responseStream.WriteAsync(reply);
        }
    }

    public override Task<GetChildrenPasswordItemIdsReply> GetChildrenPasswordItemIds(
        GetChildrenPasswordItemIdsRequest request,
        ServerCallContext context
    )
    {
        return passwordService.GetChildrenPasswordItemIdsAsync(request.Id.ToOptionGuid(), context.CancellationToken)
           .HandleAsync(
                serializer,
                ids =>
                {
                    var reply = new GetChildrenPasswordItemIdsReply();
                    reply.Ids.AddRange(ids.ToByteString().ToArray());

                    return reply;
                },
                context.CancellationToken
            );
    }
}