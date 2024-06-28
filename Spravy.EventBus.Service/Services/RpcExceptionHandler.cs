using System.Runtime.CompilerServices;
using Grpc.Core;
using Spravy.Core.Interfaces;
using Spravy.Db.Errors;
using Spravy.Domain.Errors;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.EventBus.Service.Services;

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
        var errors = new List<Error>();

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
            errors.AddRange(values.ToArray());
        }

        if (errors.Any())
        {
            return new(errors.ToArray());
        }

        return Result.Success;
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
            var error = await serializer.DeserializeAsync<UserWithEmailExistsError>(stream, ct);

            if (error.IsHasError)
            {
                return error.Errors;
            }

            return new([error.Value,]);
        }

        if (UnknownError.MainId == id)
        {
            var error = await serializer.DeserializeAsync<UnknownError>(stream, ct);

            if (error.IsHasError)
            {
                return error.Errors;
            }

            return new([error.Value,]);
        }

        if (UserWithLoginExistsError.MainId == id)
        {
            var error = await serializer.DeserializeAsync<UserWithLoginExistsError>(stream, ct);

            if (error.IsHasError)
            {
                return error.Errors;
            }

            return new([error.Value,]);
        }

        if (CastError.MainId == id)
        {
            var error = await serializer.DeserializeAsync<CastError>(stream, ct);

            if (error.IsHasError)
            {
                return error.Errors;
            }

            return new([error.Value,]);
        }

        if (ServiceUnavailableError.MainId == id)
        {
            var error = await serializer.DeserializeAsync<ServiceUnavailableError>(stream, ct);

            if (error.IsHasError)
            {
                return error.Errors;
            }

            return new([error.Value,]);
        }

        if (DefaultCtorResultError.MainId == id)
        {
            var error = await serializer.DeserializeAsync<DefaultCtorResultError>(stream, ct);

            if (error.IsHasError)
            {
                return error.Errors;
            }

            return new([error.Value,]);
        }

        if (CanceledByUserError.MainId == id)
        {
            var error = await serializer.DeserializeAsync<CanceledByUserError>(stream, ct);

            if (error.IsHasError)
            {
                return error.Errors;
            }

            return new([error.Value,]);
        }

        if (UserWithLoginNotExistsError.MainId == id)
        {
            var error = await serializer.DeserializeAsync<UserWithLoginNotExistsError>(stream, ct);

            if (error.IsHasError)
            {
                return error.Errors;
            }

            return new([error.Value,]);
        }

        if (MultiUsersWithSameLoginError.MainId == id)
        {
            var error = await serializer.DeserializeAsync<MultiUsersWithSameLoginError>(stream, ct);

            if (error.IsHasError)
            {
                return error.Errors;
            }

            return new([error.Value,]);
        }

        if (UserNotVerifiedError.MainId == id)
        {
            var error = await serializer.DeserializeAsync<UserNotVerifiedError>(stream, ct);

            if (error.IsHasError)
            {
                return error.Errors;
            }

            return new([error.Value,]);
        }

        if (PropertyNullValueError.MainId == id)
        {
            var error = await serializer.DeserializeAsync<PropertyNullValueError>(stream, ct);

            if (error.IsHasError)
            {
                return error.Errors;
            }

            return new([error.Value,]);
        }

        if (PropertyEmptyStringError.MainId == id)
        {
            var error = await serializer.DeserializeAsync<PropertyEmptyStringError>(stream, ct);

            if (error.IsHasError)
            {
                return error.Errors;
            }

            return new([error.Value,]);
        }

        if (PropertyWhiteSpaceStringError.MainId == id)
        {
            var error = await serializer.DeserializeAsync<PropertyWhiteSpaceStringError>(
                stream,
                ct
            );

            if (error.IsHasError)
            {
                return error.Errors;
            }

            return new([error.Value,]);
        }

        if (VariableNullValueError.MainId == id)
        {
            var error = await serializer.DeserializeAsync<VariableNullValueError>(stream, ct);

            if (error.IsHasError)
            {
                return error.Errors;
            }

            return new([error.Value,]);
        }

        if (VariableStringMaxLengthError.MainId == id)
        {
            var error = await serializer.DeserializeAsync<VariableStringMaxLengthError>(stream, ct);

            if (error.IsHasError)
            {
                return error.Errors;
            }

            return new([error.Value,]);
        }

        if (VariableStringMinLengthError.MainId == id)
        {
            var error = await serializer.DeserializeAsync<VariableStringMinLengthError>(stream, ct);

            if (error.IsHasError)
            {
                return error.Errors;
            }

            return new([error.Value,]);
        }

        if (VariableInvalidCharsError.MainId == id)
        {
            var error = await serializer.DeserializeAsync<VariableInvalidCharsError>(stream, ct);

            if (error.IsHasError)
            {
                return error.Errors;
            }

            return new([error.Value,]);
        }

        if (NotFoundNamedError.MainId == id)
        {
            var error = await serializer.DeserializeAsync<NotFoundNamedError>(stream, ct);

            if (error.IsHasError)
            {
                return error.Errors;
            }

            return new([error.Value,]);
        }

        if (EmptyArrayError.MainId == id)
        {
            var error = await serializer.DeserializeAsync<EmptyArrayError>(stream, ct);

            if (error.IsHasError)
            {
                return error.Errors;
            }

            return new([error.Value,]);
        }

        if (MultiUsersWithSameEmailError.MainId == id)
        {
            var error = await serializer.DeserializeAsync<MultiUsersWithSameEmailError>(stream, ct);

            if (error.IsHasError)
            {
                return error.Errors;
            }

            return new([error.Value,]);
        }

        if (MultiValuesArrayError.MainId == id)
        {
            var error = await serializer.DeserializeAsync<MultiValuesArrayError>(stream, ct);

            if (error.IsHasError)
            {
                return error.Errors;
            }

            return new([error.Value,]);
        }

        if (UserWithEmailNotExistsError.MainId == id)
        {
            var error = await serializer.DeserializeAsync<UserWithEmailNotExistsError>(stream, ct);

            if (error.IsHasError)
            {
                return error.Errors;
            }

            return new([error.Value,]);
        }

        if (RoleOutOfRangeError.MainId == id)
        {
            var error = await serializer.DeserializeAsync<RoleOutOfRangeError>(stream, ct);

            if (error.IsHasError)
            {
                return error.Errors;
            }

            return new([error.Value,]);
        }

        if (ContinueError.MainId == id)
        {
            var error = await serializer.DeserializeAsync<ContinueError>(stream, ct);

            if (error.IsHasError)
            {
                return error.Errors;
            }

            return new([error.Value,]);
        }

        if (MaxCycleCountReachedError.MainId == id)
        {
            var error = await serializer.DeserializeAsync<MaxCycleCountReachedError>(stream, ct);

            if (error.IsHasError)
            {
                return error.Errors;
            }

            return new([error.Value,]);
        }

        if (NotFoundTypeError.MainId == id)
        {
            var error = await serializer.DeserializeAsync<NotFoundTypeError>(stream, ct);

            if (error.IsHasError)
            {
                return error.Errors;
            }

            return new([error.Value,]);
        }

        if (NotFoundEntityError.MainId == id)
        {
            var error = await serializer.DeserializeAsync<NotFoundEntityError>(stream, ct);

            if (error.IsHasError)
            {
                return error.Errors;
            }

            return new([error.Value,]);
        }

        return new([new NotFoundError(id),]);
    }
}
