using Google.Protobuf;
using Riok.Mapperly.Abstractions;
using Spravy.Core.Mappers;
using Spravy.PasswordGenerator.Domain.Models;
using Spravy.PasswordGenerator.Protos;

namespace Spravy.PasswordGenerator.Domain.Mapper.Mappers;

[Mapper(PreferParameterlessConstructors = false)]
public static partial class PasswordGeneratorMapper
{
    public static partial AddPasswordOptions ToAddPasswordOptions(this AddPasswordItemRequest request);

    public static partial AddPasswordItemRequest ToAddPasswordItemRequest(this AddPasswordOptions request);

    public static partial PasswordItem ToPasswordItem(this PasswordItemGrpc request);

    public static partial PasswordItemGrpc ToPasswordItemGrpc(this PasswordItem request);

    public static partial PasswordItem ToPasswordItem(this GetPasswordItemReply request);

    public static partial GetPasswordItemReply ToGetPasswordItemReply(this PasswordItem request);

    public static partial ReadOnlyMemory<PasswordItem> ToPasswordItem(this IEnumerable<PasswordItemGrpc> request);

    public static partial ReadOnlyMemory<PasswordItemGrpc> ToPasswordItemGrpc(
        this ReadOnlyMemory<PasswordItem> request
    );

    private static Guid ToGuid(ByteString byteString)
    {
        return byteString.ToGuid();
    }

    private static ByteString ToByteString(Guid id)
    {
        return id.ToByteString();
    }
}