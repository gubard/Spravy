using Google.Protobuf;
using Riok.Mapperly.Abstractions;
using Spravy.Core.Mappers;
using Spravy.Domain.Models;
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
    public static partial EditPropertyUInt32Grpc ToEditPropertyUInt32Grpc(this EditPropertyValue<ushort> value);
    public static partial EditPropertyBooleanGrpc ToEditPropertyBooleanGrpc(this EditPropertyValue<bool> value);

    public static partial ReadOnlyMemory<PasswordItemGrpc> ToPasswordItemGrpc(
        this ReadOnlyMemory<PasswordItem> request
    );

    public static EditPasswordItemsGrpc ToEditPasswordItemsGrpc(this EditPasswordItems options)
    {
        var result = new EditPasswordItemsGrpc
        {
            Key = options.Key.ToEditPropertyStringGrpc(),
            Name = options.Name.ToEditPropertyStringGrpc(),
            Length = options.Length.ToEditPropertyUInt32Grpc(),
            Login = options.Login.ToEditPropertyStringGrpc(),
            Regex = options.Regex.ToEditPropertyStringGrpc(),
            CustomAvailableCharacters = options.CustomAvailableCharacters.ToEditPropertyStringGrpc(),
            IsAvailableNumber = options.IsAvailableNumber.ToEditPropertyBooleanGrpc(),
            IsAvailableLowerLatin = options.IsAvailableLowerLatin.ToEditPropertyBooleanGrpc(),
            IsAvailableSpecialSymbols = options.IsAvailableSpecialSymbols.ToEditPropertyBooleanGrpc(),
            IsAvailableUpperLatin = options.IsAvailableUpperLatin.ToEditPropertyBooleanGrpc(),
        };

        result.Ids.AddRange(options.Ids.ToByteString().ToArray());

        return result;
    }

    public static EditPasswordItems ToEditPasswordItems(this EditPasswordItemsGrpc value)
    {
        return new(
            value.Ids.ToGuid(),
            value.Name.IsEdit ? new EditPropertyValue<string>(value.Name.Value) : new(),
            value.Login.IsEdit ? new EditPropertyValue<string>(value.Login.Value) : new(),
            value.Key.IsEdit ? new EditPropertyValue<string>(value.Key.Value) : new(),
            value.CustomAvailableCharacters.IsEdit
                ? new EditPropertyValue<string>(value.CustomAvailableCharacters.Value)
                : new(),
            value.Regex.IsEdit ? new EditPropertyValue<string>(value.Regex.Value) : new(),
            value.Length.IsEdit ? new EditPropertyValue<ushort>((ushort)value.Length.Value) : new(),
            value.IsAvailableUpperLatin.IsEdit ? new EditPropertyValue<bool>(value.IsAvailableUpperLatin.Value) : new(),
            value.IsAvailableLowerLatin.IsEdit ? new EditPropertyValue<bool>(value.IsAvailableLowerLatin.Value) : new(),
            value.IsAvailableNumber.IsEdit ? new EditPropertyValue<bool>(value.IsAvailableNumber.Value) : new(),
            value.IsAvailableSpecialSymbols.IsEdit
                ? new EditPropertyValue<bool>(value.IsAvailableSpecialSymbols.Value)
                : new()
        );
    }

    public static EditPropertyStringGrpc ToEditPropertyStringGrpc(this EditPropertyValue<string> value)
    {
        if (value.IsEdit)
        {
            return new()
            {
                IsEdit = true,
                Value = value.Value,
            };
        }

        return new()
        {
            IsEdit = false,
            Value = string.Empty,
        };
    }

    private static Guid ToGuid(ByteString byteString)
    {
        return byteString.ToGuid();
    }

    private static ByteString ToByteString(Guid id)
    {
        return id.ToByteString();
    }
}