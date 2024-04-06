using System.Runtime.CompilerServices;
using ProtoBuf;
using ProtoBuf.Meta;
using Spravy.Domain.Errors;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.Core.Services;

public class ProtobufSerializer : ISerializer
{
    static ProtobufSerializer()
    {
        RuntimeTypeModel.Default.Case(
                rtm => rtm.Add<UserWithLoginExistsError>()
                    .Case(mt => mt.AddField(1, nameof(UserWithLoginExistsError.Id)))
                    .Case(mt => mt.AddField(2, nameof(UserWithLoginExistsError.Name)))
            )
            .Case(
                rtm => rtm.Add<NotNullError>()
                    .Case(mt => mt.AddField(1, nameof(NotNullError.Id)))
                    .Case(mt => mt.AddField(2, nameof(NotNullError.Name)))
            )
            .Case(
                rtm => rtm.Add<StringMaxLengthError>()
                    .Case(mt => mt.AddField(1, nameof(StringMaxLengthError.Id)))
                    .Case(mt => mt.AddField(2, nameof(StringMaxLengthError.Name)))
                    .Case(mt => mt.AddField(3, nameof(StringMaxLengthError.MaxLength)))
            )
            .Case(
                rtm => rtm.Add<StringMinLengthError>()
                    .Case(mt => mt.AddField(1, nameof(StringMinLengthError.Id)))
                    .Case(mt => mt.AddField(2, nameof(StringMinLengthError.Name)))
                    .Case(mt => mt.AddField(3, nameof(StringMinLengthError.MinLength)))
            )
            .Case(
                rtm => rtm.Add<UserWithEmailExistsError>()
                    .Case(mt => mt.AddField(1, nameof(UserWithEmailExistsError.Id)))
                    .Case(mt => mt.AddField(2, nameof(UserWithEmailExistsError.Name)))
            )
            .Case(
                rtm => rtm.Add<ValidCharsError>()
                    .Case(mt => mt.AddField(1, nameof(ValidCharsError.Id)))
                    .Case(mt => mt.AddField(2, nameof(ValidCharsError.Name)))
                    .Case(mt => mt.AddField(2, nameof(ValidCharsError.ValidChars)))
            );
    }

    public ConfiguredValueTaskAwaitable<Result> Serialize(object obj, Stream stream)
    {
        Serializer.Serialize(stream, obj);

        return Result.AwaitableFalse;
    }

    public ConfiguredValueTaskAwaitable<Result<TObject>> Deserialize<TObject>(Stream stream)
    {
        return Serializer.Deserialize<TObject>(stream).ToResult().ToValueTaskResult().ConfigureAwait(false);
    }
}