using System.Reflection;
using System.Runtime.CompilerServices;
using Grpc.Core;
using Spravy.Domain.Errors;
using Spravy.Domain.Exceptions;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.Client.Extensions;

public static class RpcExceptionExtension
{
    private static readonly Dictionary<Guid, Type> errors = new();
    private static readonly Dictionary<Type, Func<ISerializer, MemoryStream, Error>> chace = new();

    private static readonly MethodInfo DeserializeAsyncMethod =
        typeof(ISerializer).GetMethod(nameof(ISerializer.Deserialize)).ThrowIfNull();

    static RpcExceptionExtension()
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            LoadErrors(assembly);
        }
    }

    private static void LoadErrors(Assembly assembly)
    {
        var errorTypes = assembly.GetTypes()
            .Where(x => typeof(Error).IsAssignableFrom(x) && x is { IsAbstract: false, IsGenericType: false })
            .ToArray();

        foreach (var errorType in errorTypes)
        {
            errors.Add(
                (Guid)errorType.GetField("MainId", BindingFlags.Static | BindingFlags.Public)?.GetValue(null)
                    .ThrowIfNull(),
                errorType);
        }
    }

    public static ConfiguredValueTaskAwaitable<Result> ToErrorAsync(this RpcException exception, ISerializer serializer)
    {
        return ToErrorCore(exception, serializer).ConfigureAwait(false);
    }

    private static async ValueTask<Result> ToErrorCore(this RpcException exception, ISerializer serializer)
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
            .ToProperty(typeof(Result<>).MakeGenericType(type).GetProperty(nameof(Result<object>.Value)).ThrowIfNull())
            .ToConvert(typeof(Error))
            .ToLambda(
                [serializer, memoryStream]
            )
            .Compile();

        return chace[type];
    }

    public static Type? GetValidationResultType(Guid id)
    {
        return errors.GetValueOrDefault(id);
    }
}