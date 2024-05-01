namespace Spravy.Ui.Models;

public class NotifyBase : ReactiveObject, IDisposable
{
    protected List<IDisposable> Disposables = new();

    public void Dispose()
    {
        foreach (var disposable in Disposables)
        {
            disposable.Dispose();
        }

        Disposables.Clear();
    }
}