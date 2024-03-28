using System.Reflection;
using Grpc.Core;
using Spravy.Domain.Exceptions;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.Client.Extensions;

public static class RpcExceptionExtension
{
    private static readonly Dictionary<Type, Func<ISerializer, MemoryStream, ValidationResult>> chace = new();

    private static readonly MethodInfo DeserializeAsyncMethod =
        typeof(ISerializer).GetMethod(nameof(ISerializer.Deserialize)).ThrowIfNull();

    public static async Task<Error> ToErrorAsync(this RpcException exception, ISerializer serializer)
    {
        var validationResults = new List<ValidationResult>();

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
                validationResults.Add(new UnknownValidationResult(guid));

                continue;
            }

            var func = GetFunc(type);
            await using var stream = new MemoryStream();
            var validationResult = func.Invoke(serializer, stream);
            validationResults.Add(validationResult);
        }

        if (validationResults.Any())
        {
            return new Error(validationResults.ToArray());
        }

        return new Error();
    }

    public static Func<ISerializer, MemoryStream, ValidationResult> GetFunc(Type type)
    {
        if (chace.TryGetValue(type, out var func))
        {
            return func;
        }

        var serializer = typeof(ISerializer).ToParameter();
        var memoryStream = typeof(MemoryStream).ToParameter();

        chace[type] = (Func<ISerializer, MemoryStream, ValidationResult>)DeserializeAsyncMethod
            .MakeGenericMethod(type)
            .ToCall(serializer, memoryStream)
            .ToConvert(typeof(ValidationResult))
            .ToLambda(
                [serializer, memoryStream]
            )
            .Compile();

        return chace[type];
    }


    public static Type? GetValidationResultType(Guid id)
    {
        if (NotNullValidationResult.MainId == id)
        {
            return typeof(NotNullValidationResult);
        }

        if (StringMaxLengthValidationResult.MainId == id)
        {
            return typeof(StringMaxLengthValidationResult);
        }

        if (StringMinLengthValidationResult.MainId == id)
        {
            return typeof(StringMinLengthValidationResult);
        }

        if (UserWithEmailExistsValidationResult.MainId == id)
        {
            return typeof(UserWithEmailExistsValidationResult);
        }

        if (UserWithLoginExistsValidationResult.MainId == id)
        {
            return typeof(UserWithLoginExistsValidationResult);
        }

        if (ValidCharsValidationResult.MainId == id)
        {
            return typeof(ValidCharsValidationResult);
        }

        return null;
    }
}