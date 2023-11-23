namespace Spravy.Domain.Services;

public class CancellationTokenFactory
{
    private CancellationTokenSource cancellationTokenSource = new();

    public CancellationToken Token => cancellationTokenSource.Token;

    public void Reset()
    {
        cancellationTokenSource = new();
    }

    public void Cancel()
    {
        cancellationTokenSource.Cancel();
    }
}