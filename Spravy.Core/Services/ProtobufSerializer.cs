using System.Reflection;
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
    public static void LoadErrors(Assembly assembly)
    {
        var errorTypes = assembly.GetTypes()
            .Where(x => typeof(Error).IsAssignableFrom(x) && x is { IsAbstract: false, IsGenericType: false })
            .ToArray();

        foreach (var errorType in errorTypes)
        {
            var metaType = RuntimeTypeModel.Default.Add(errorType);
            var fields = errorType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            for (var i = 0; i < fields.Length; i++)
            {
                metaType.AddField(i + 1, fields[i].Name);
            }
        }
    }

    public ConfiguredValueTaskAwaitable<Result> SerializeAsync(object obj, Stream stream)
    {
        Serializer.Serialize(stream, obj);

        return Result.AwaitableFalse;
    }

    public ConfiguredValueTaskAwaitable<Result<TObject>> DeserializeAsync<TObject>(Stream stream)
    {
        return Deserialize<TObject>(stream).ToValueTaskResult().ConfigureAwait(false);
    }

    public Result<TObject> Deserialize<TObject>(Stream stream)
    {
        return Serializer.Deserialize<TObject>(stream).ToResult();
    }
}