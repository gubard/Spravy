using System.Runtime.CompilerServices;
using Grpc.Core;
using Spravy.Core.Interfaces;
using Spravy.Db.Errors;
using Spravy.Domain.Errors;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.Authentication.Service.Services;

public class RpcExceptionHandler : IRpcExceptionHandler
{
    private readonly ISerializer serializer;

    public RpcExceptionHandler(ISerializer serializer)
    {
        this.serializer = serializer;
    }

    public ConfiguredValueTaskAwaitable<Result> ToErrorAsync(
        RpcException exception,
        CancellationToken ct
    )
    {
        return ToErrorCore(exception, ct).ConfigureAwait(false);
    }

    private async ValueTask<Result> ToErrorCore(RpcException exception, CancellationToken ct)
    {
        var errors = new ReadOnlyMemory<Error>();

        foreach (var trailer in exception.Trailers)
        {
            if (!trailer.Key.EndsWith("-bin"))
            {
                continue;
            }

            var key = trailer.Key.Substring(0, trailer.Key.Length - 4);

            if (!Guid.TryParse(key, out var id))
            {
                continue;
            }

            var values = await GetErrorsAsync(id, trailer, ct);
            errors = errors.Combine(values);
        }

        return !errors.IsEmpty ? new(errors) : Result.Success;
    }

    private async ValueTask<ReadOnlyMemory<Error>> GetErrorsAsync(
        Guid id,
        Metadata.Entry trailer,
        CancellationToken ct
    )
    {
        await using var stream = new MemoryStream(trailer.ValueBytes);

        if (UserWithEmailExistsError.MainId == id)
        {
            var result = await serializer.DeserializeAsync<UserWithEmailExistsError>(stream, ct);

            if (!result.TryGetValue(out var value))
            {
                return result.Errors;
            }

            return new([value,]);
        }

        if (UnknownError.MainId == id)
        {
            var result = await serializer.DeserializeAsync<UnknownError>(stream, ct);

            if (!result.TryGetValue(out var value))
            {
                return result.Errors;
            }

            return new([value,]);
        }

        if (UserWithLoginExistsError.MainId == id)
        {
            var result = await serializer.DeserializeAsync<UserWithLoginExistsError>(stream, ct);

            if (!result.TryGetValue(out var value))
            {
                return result.Errors;
            }

            return new([value,]);
        }

        if (CastError.MainId == id)
        {
            var result = await serializer.DeserializeAsync<CastError>(stream, ct);

            if (!result.TryGetValue(out var value))
            {
                return result.Errors;
            }

            return new([value,]);
        }

        if (ServiceUnavailableError.MainId == id)
        {
            var result = await serializer.DeserializeAsync<ServiceUnavailableError>(stream, ct);

            if (!result.TryGetValue(out var value))
            {
                return result.Errors;
            }

            return new([value,]);
        }

        if (DefaultCtorResultError.MainId == id)
        {
            var result = await serializer.DeserializeAsync<DefaultCtorResultError>(stream, ct);

            if (!result.TryGetValue(out var value))
            {
                return result.Errors;
            }

            return new([value,]);
        }

        if (CanceledByUserError.MainId == id)
        {
            var result = await serializer.DeserializeAsync<CanceledByUserError>(stream, ct);

            if (!result.TryGetValue(out var value))
            {
                return result.Errors;
            }

            return new([value,]);
        }

        if (UserWithLoginNotExistsError.MainId == id)
        {
            var result = await serializer.DeserializeAsync<UserWithLoginNotExistsError>(stream, ct);

            if (!result.TryGetValue(out var value))
            {
                return result.Errors;
            }

            return new([value,]);
        }

        if (MultiUsersWithSameLoginError.MainId == id)
        {
            var result = await serializer.DeserializeAsync<MultiUsersWithSameLoginError>(
                stream,
                ct
            );

            if (!result.TryGetValue(out var value))
            {
                return result.Errors;
            }

            return new([value,]);
        }

        if (UserNotVerifiedError.MainId == id)
        {
            var result = await serializer.DeserializeAsync<UserNotVerifiedError>(stream, ct);

            if (!result.TryGetValue(out var value))
            {
                return result.Errors;
            }

            return new([value,]);
        }

        if (PropertyNullValueError.MainId == id)
        {
            var result = await serializer.DeserializeAsync<PropertyNullValueError>(stream, ct);

            if (!result.TryGetValue(out var value))
            {
                return result.Errors;
            }

            return new([value,]);
        }

        if (PropertyEmptyStringError.MainId == id)
        {
            var result = await serializer.DeserializeAsync<PropertyEmptyStringError>(stream, ct);

            if (!result.TryGetValue(out var value))
            {
                return result.Errors;
            }

            return new([value,]);
        }

        if (PropertyWhiteSpaceStringError.MainId == id)
        {
            var result = await serializer.DeserializeAsync<PropertyWhiteSpaceStringError>(
                stream,
                ct
            );

            if (!result.TryGetValue(out var value))
            {
                return result.Errors;
            }

            return new([value,]);
        }

        if (VariableNullValueError.MainId == id)
        {
            var result = await serializer.DeserializeAsync<VariableNullValueError>(stream, ct);

            if (!result.TryGetValue(out var value))
            {
                return result.Errors;
            }

            return new([value,]);
        }

        if (VariableStringMaxLengthError.MainId == id)
        {
            var result = await serializer.DeserializeAsync<VariableStringMaxLengthError>(
                stream,
                ct
            );

            if (!result.TryGetValue(out var value))
            {
                return result.Errors;
            }

            return new([value,]);
        }

        if (VariableStringMinLengthError.MainId == id)
        {
            var result = await serializer.DeserializeAsync<VariableStringMinLengthError>(
                stream,
                ct
            );

            if (!result.TryGetValue(out var value))
            {
                return result.Errors;
            }

            return new([value,]);
        }

        if (VariableInvalidCharsError.MainId == id)
        {
            var result = await serializer.DeserializeAsync<VariableInvalidCharsError>(stream, ct);

            if (!result.TryGetValue(out var value))
            {
                return result.Errors;
            }

            return new([value,]);
        }

        if (NotFoundNamedError.MainId == id)
        {
            var result = await serializer.DeserializeAsync<NotFoundNamedError>(stream, ct);

            if (!result.TryGetValue(out var value))
            {
                return result.Errors;
            }

            return new([value,]);
        }

        if (EmptyArrayError.MainId == id)
        {
            var result = await serializer.DeserializeAsync<EmptyArrayError>(stream, ct);

            if (!result.TryGetValue(out var value))
            {
                return result.Errors;
            }

            return new([value,]);
        }

        if (MultiUsersWithSameEmailError.MainId == id)
        {
            var result = await serializer.DeserializeAsync<MultiUsersWithSameEmailError>(
                stream,
                ct
            );

            if (!result.TryGetValue(out var value))
            {
                return result.Errors;
            }

            return new([value,]);
        }

        if (MultiValuesArrayError.MainId == id)
        {
            var result = await serializer.DeserializeAsync<MultiValuesArrayError>(stream, ct);

            if (!result.TryGetValue(out var value))
            {
                return result.Errors;
            }

            return new([value,]);
        }

        if (UserWithEmailNotExistsError.MainId == id)
        {
            var result = await serializer.DeserializeAsync<UserWithEmailNotExistsError>(stream, ct);

            if (!result.TryGetValue(out var value))
            {
                return result.Errors;
            }

            return new([value,]);
        }

        if (RoleOutOfRangeError.MainId == id)
        {
            var result = await serializer.DeserializeAsync<RoleOutOfRangeError>(stream, ct);

            if (!result.TryGetValue(out var value))
            {
                return result.Errors;
            }

            return new([value,]);
        }

        if (ContinueError.MainId == id)
        {
            var result = await serializer.DeserializeAsync<ContinueError>(stream, ct);

            if (!result.TryGetValue(out var value))
            {
                return result.Errors;
            }

            return new([value,]);
        }

        if (MaxCycleCountReachedError.MainId == id)
        {
            var result = await serializer.DeserializeAsync<MaxCycleCountReachedError>(stream, ct);

            if (!result.TryGetValue(out var value))
            {
                return result.Errors;
            }

            return new([value,]);
        }

        if (NotFoundTypeError.MainId == id)
        {
            var result = await serializer.DeserializeAsync<NotFoundTypeError>(stream, ct);

            if (!result.TryGetValue(out var value))
            {
                return result.Errors;
            }

            return new([value,]);
        }

        if (NotFoundError.MainId == id)
        {
            var result = await serializer.DeserializeAsync<NotFoundError>(stream, ct);

            if (!result.TryGetValue(out var value))
            {
                return result.Errors;
            }

            return new([value,]);
        }

        if (NotFoundEntityError.MainId == id)
        {
            var result = await serializer.DeserializeAsync<NotFoundEntityError>(stream, ct);

            if (!result.TryGetValue(out var value))
            {
                return result.Errors;
            }

            return new([value,]);
        }

        return new([new NotFoundError(id),]);
    }
}
