namespace Spravy.Client.Extensions;

public static class RpcExceptionExtension
{
    private static readonly Dictionary<Guid, Type> errorTypes = new();
    private static readonly Dictionary<Type, Func<ISerializer, MemoryStream, Error>> cache = new();
    
    private static readonly MethodInfo deserializeAsyncMethod =
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
           .Where(x => typeof(Error).IsAssignableFrom(x) && x is { IsAbstract: false, IsGenericType: false, })
           .ToArray();
        
        foreach (var errorType in errorTypes)
        {
            var field = errorType.GetField("MainId", BindingFlags.Static | BindingFlags.Public).ThrowIfNull();
            var fieldValue = field.GetValue(null).ThrowIfNull();
            RpcExceptionExtension.errorTypes.Add((Guid)fieldValue, errorType);
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
            await using var stream = new MemoryStream(trailer.ValueBytes);
            var validationResult = func.Invoke(serializer, stream);
            errors.Add(validationResult);
        }
        
        if (errors.Any())
        {
            return new(errors.ToArray());
        }
        
        return Result.Success;
    }
    
    public static Func<ISerializer, MemoryStream, Error> GetFunc(Type type)
    {
        if (cache.TryGetValue(type, out var func))
        {
            return func;
        }
        
        var serializer = typeof(ISerializer).ToParameter();
        var memoryStream = typeof(MemoryStream).ToParameter();
        
        cache[type] = (Func<ISerializer, MemoryStream, Error>)deserializeAsyncMethod.MakeGenericMethod(type)
           .ToCall(serializer, memoryStream)
           .ToProperty(typeof(Result<>).MakeGenericType(type).GetProperty(nameof(Result<object>.Value)).ThrowIfNull())
           .ToConvert(typeof(Error))
           .ToLambda([serializer, memoryStream,])
           .Compile();
        
        return cache[type];
    }
    
    public static Type? GetValidationResultType(Guid id)
    {
        return errorTypes.GetValueOrDefault(id);
    }
}