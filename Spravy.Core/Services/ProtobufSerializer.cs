using ProtoBuf;
using ProtoBuf.Meta;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.Domain.ValidationResults;

namespace Spravy.Core.Services;

public class ProtobufSerializer : ISerializer
{
    static ProtobufSerializer()
    {
        RuntimeTypeModel.Default.Case(
                rtm => rtm.Add<UserWithLoginExistsValidationResult>()
                    .Case(mt => mt.AddField(1, nameof(UserWithLoginExistsValidationResult.Id)))
                    .Case(mt => mt.AddField(2, nameof(UserWithLoginExistsValidationResult.Name)))
            )
            .Case(
                rtm => rtm.Add<NotNullValidationResult>()
                    .Case(mt => mt.AddField(1, nameof(NotNullValidationResult.Id)))
                    .Case(mt => mt.AddField(2, nameof(NotNullValidationResult.Name)))
            )
            .Case(
                rtm => rtm.Add<StringMaxLengthValidationResult>()
                    .Case(mt => mt.AddField(1, nameof(StringMaxLengthValidationResult.Id)))
                    .Case(mt => mt.AddField(2, nameof(StringMaxLengthValidationResult.Name)))
                    .Case(mt => mt.AddField(3, nameof(StringMaxLengthValidationResult.MaxLength)))
            )
            .Case(
                rtm => rtm.Add<StringMinLengthValidationResult>()
                    .Case(mt => mt.AddField(1, nameof(StringMinLengthValidationResult.Id)))
                    .Case(mt => mt.AddField(2, nameof(StringMinLengthValidationResult.Name)))
                    .Case(mt => mt.AddField(3, nameof(StringMinLengthValidationResult.MinLength)))
            )
            .Case(
                rtm => rtm.Add<UserWithEmailExistsValidationResult>()
                    .Case(mt => mt.AddField(1, nameof(UserWithEmailExistsValidationResult.Id)))
                    .Case(mt => mt.AddField(2, nameof(UserWithEmailExistsValidationResult.Name)))
            )
            .Case(
                rtm => rtm.Add<ValidCharsValidationResult>()
                    .Case(mt => mt.AddField(1, nameof(ValidCharsValidationResult.Id)))
                    .Case(mt => mt.AddField(2, nameof(ValidCharsValidationResult.Name)))
                    .Case(mt => mt.AddField(2, nameof(ValidCharsValidationResult.ValidChars)))
            );
    }

    public void Serialize(object obj, Stream stream)
    {
        Serializer.Serialize(stream, obj);
    }

    public TObject Deserialize<TObject>(Stream stream)
    {
        return Serializer.Deserialize<TObject>(stream);
    }
}