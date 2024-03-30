using System.Reflection;
using Grpc.Core;
using Spravy.Domain.Exceptions;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.Domain.ValidationResults;

namespace Spravy.Client.Extensions;

public static class RpcExceptionExtension
{
    private static readonly Dictionary<Type, Func<ISerializer, MemoryStream, Error>> chace = new();

    private static readonly MethodInfo DeserializeAsyncMethod =
        typeof(ISerializer).GetMethod(nameof(ISerializer.Deserialize)).ThrowIfNull();

    public static async Task<Result> ToErrorAsync(this RpcException exception, ISerializer serializer)
    {
        var errors = new List<Error>();

        foreach (var trailer in exception.Trailers)
        {
            if (!trailer.Key.EndsWith("-bin"))
            {
                continue;
            }

            var key = trailer.Key.Substring(0, trailer.Key.Length - 4);

            if (!Guid.TryParse(key, out var guid))
            {
                continue;
            }

            var type = GetValidationResultType(guid);

            if (type is null)
            {
                errors.Add(new UnknownError(guid));

                continue;
            }

            var func = GetFunc(type);
            await using var stream = new MemoryStream();
            var validationResult = func.Invoke(serializer, stream);
            errors.Add(validationResult);
        }

        if (errors.Any())
        {
            return new Result(errors.ToArray());
        }

        return Result.Success;
    }

    public static Func<ISerializer, MemoryStream, Error> GetFunc(Type type)
    {
        if (chace.TryGetValue(type, out var func))
        {
            return func;
        }

        var serializer = typeof(ISerializer).ToParameter();
        var memoryStream = typeof(MemoryStream).ToParameter();

        chace[type] = (Func<ISerializer, MemoryStream, Error>)DeserializeAsyncMethod
            .MakeGenericMethod(type)
            .ToCall(serializer, memoryStream)
            .ToConvert(typeof(Error))
            .ToLambda(
                [serializer, memoryStream]
            )
            .Compile();

        return chace[type];
    }


    public static Type? GetValidationResultType(Guid id)
    {
        if (NotNullError.MainId == id)
        {
            return typeof(NotNullError);
        }

        if (StringMaxLengthError.MainId == id)
        {
            return typeof(StringMaxLengthError);
        }

        if (StringMinLengthError.MainId == id)
        {
            return typeof(StringMinLengthError);
        }

        if (UserWithEmailExistsError.MainId == id)
        {
            return typeof(UserWithEmailExistsError);
        }

        if (UserWithLoginExistsError.MainId == id)
        {
            return typeof(UserWithLoginExistsError);
        }

        if (ValidCharsError.MainId == id)
        {
            return typeof(ValidCharsError);
        }

        return null;
    }
}