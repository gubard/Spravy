namespace Spravy.Client.Services;

public class GrpcChannelCacheValidator : ICacheValidator<Uri, GrpcChannel>
{
    public bool IsValid(Uri key, GrpcChannel value)
    {
        return value.State switch
        {
            ConnectivityState.Idle => true,
            ConnectivityState.Connecting => true,
            ConnectivityState.Ready => true,
            ConnectivityState.TransientFailure => false,
            ConnectivityState.Shutdown => false,
            _ => throw new ArgumentOutOfRangeException(nameof(value.State), value.State, null),
        };
    }
}