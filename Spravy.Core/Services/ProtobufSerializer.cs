namespace Spravy.Core.Services;

public class ProtobufSerializer : ISerializer
{
    static ProtobufSerializer()
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            LoadErrors(assembly);
        }
    }

    public ConfiguredValueTaskAwaitable<Result> SerializeAsync<T>(T obj, Stream stream, CancellationToken ct)
        where T : notnull
    {
        Serializer.Serialize(stream, obj);

        return Result.AwaitableSuccess;
    }

    public ConfiguredValueTaskAwaitable<Result<TObject>> DeserializeAsync<TObject>(Stream stream, CancellationToken ct)
        where TObject : notnull
    {
        return Deserialize<TObject>(stream).ToValueTaskResult().ConfigureAwait(false);
    }

    public Result<TObject> Deserialize<TObject>(Stream stream) where TObject : notnull
    {
        return Serializer.Deserialize<TObject>(stream).ToResult();
    }

    private static void LoadErrors(Assembly assembly)
    {
        var errorTypes = assembly.GetTypes()
           .Where(x => typeof(Error).IsAssignableFrom(x) && x is { IsAbstract: false, IsGenericType: false, })
           .ToArray();

        foreach (var errorType in errorTypes)
        {
            var defaultConstructor =
                errorType.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, [], null)
             ?? errorType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, [], null);

            if (defaultConstructor is null)
            {
                throw new NullReferenceException();
            }

            var metaType = RuntimeTypeModel.Default.Add(errorType);

            var fields = errorType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
               .Where(x => x.CanWrite)
               .ToArray();

            for (var i = 0; i < fields.Length; i++)
            {
                metaType.AddField(i + 1, fields[i].Name);
            }
        }
    }
}