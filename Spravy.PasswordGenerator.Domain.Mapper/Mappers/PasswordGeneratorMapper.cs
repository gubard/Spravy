using Google.Protobuf;
using Riok.Mapperly.Abstractions;
using Spravy.Core.Mappers;
using Spravy.Domain.Models;
using Spravy.PasswordGenerator.Domain.Enums;
using Spravy.PasswordGenerator.Domain.Models;
using Spravy.PasswordGenerator.Protos;

namespace Spravy.PasswordGenerator.Domain.Mapper.Mappers;

[Mapper(PreferParameterlessConstructors = false)]
public static partial class PasswordGeneratorMapper
{
    public static partial ReadOnlyMemory<PasswordItem> ToPasswordItem(this IEnumerable<PasswordItemGrpc> request);
    public static partial EditPropertyUInt32Grpc ToEditPropertyUInt32Grpc(this EditPropertyValue<ushort> value);
    public static partial EditPropertyBooleanGrpc ToEditPropertyBooleanGrpc(this EditPropertyValue<bool> value);

    public static partial ReadOnlyMemory<PasswordItemGrpc> ToPasswordItemGrpc(
        this ReadOnlyMemory<PasswordItem> request
    );
    
    public static EditPropertyPasswordItemTypeGrpc ToEditPropertyPasswordItemTypeGrpc(
        this EditPropertyValue<PasswordItemType> value
    )
    {
        return new()
        {
            IsEdit = value.IsEdit,
            Value = (PasswordItemTypeGrpc)value.Value,
        };
    }

    public static AddPasswordOptions ToAddPasswordOptions(this AddPasswordItemRequest request)
    {
        return new(
            request.Name,
            request.Key,
            (ushort)request.Length,
            request.Regex,
            request.IsAvailableLowerLatin,
            request.IsAvailableUpperLatin,
            request.IsAvailableNumber,
            request.IsAvailableSpecialSymbols,
            request.CustomAvailableCharacters,
            request.Login,
            (PasswordItemType)request.Type
        );
    }

    public static AddPasswordItemRequest ToAddPasswordItemRequest(this AddPasswordOptions value)
    {
        return new()
        {
            IsAvailableLowerLatin = value.IsAvailableLowerLatin,
            IsAvailableSpecialSymbols = value.IsAvailableSpecialSymbols,
            IsAvailableUpperLatin = value.IsAvailableUpperLatin,
            CustomAvailableCharacters = value.CustomAvailableCharacters,
            IsAvailableNumber = value.IsAvailableNumber,
            Key = value.Key,
            Length = value.Length,
            Login = value.Login,
            Name = value.Name,
            Regex = value.Regex,
            Type = (PasswordItemTypeGrpc)value.Type,
        };
    }

    public static PasswordItem ToPasswordItem(this PasswordItemGrpc request)
    {
        return new(
            request.Id.ToGuid(),
            request.Name,
            request.Key,
            (ushort)request.Length,
            request.Regex,
            request.IsAvailableUpperLatin,
            request.IsAvailableLowerLatin,
            request.IsAvailableSpecialSymbols,
            request.IsAvailableNumber,
            request.CustomAvailableCharacters,
            request.Login,
            (PasswordItemType)request.Type,
            request.OrderIndex
        );
    }

    public static PasswordItemGrpc ToPasswordItemGrpc(this PasswordItem value)
    {
        return new()
        {
            Type = (PasswordItemTypeGrpc)value.Type,
            CustomAvailableCharacters = value.CustomAvailableCharacters,
            IsAvailableNumber = value.IsAvailableNumber,
            IsAvailableLowerLatin = value.IsAvailableLowerLatin,
            IsAvailableSpecialSymbols = value.IsAvailableSpecialSymbols,
            IsAvailableUpperLatin = value.IsAvailableUpperLatin,
            Length = value.Length,
            Regex = value.Regex,
            Key = value.Key,
            Name = value.Name,
            Login = value.Login,
            Id = value.Id.ToByteString(),
            OrderIndex = value.OrderIndex,
        };
    }

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
            Type = options.Type.ToEditPropertyPasswordItemTypeGrpc(),
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
                : new(),
            value.Type.IsEdit
                ? new EditPropertyValue<PasswordItemType>((PasswordItemType)value.Type.Value)
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