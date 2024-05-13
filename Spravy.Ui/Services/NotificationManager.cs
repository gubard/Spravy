namespace Spravy.Ui.Services;

public class NotificationManager : ISpravyNotificationManager
{
    private readonly IKernel kernel;
    private readonly IManagedNotificationManager managedNotificationManager;

    public NotificationManager(IManagedNotificationManager managedNotificationManager, IKernel kernel)
    {
        this.managedNotificationManager = managedNotificationManager;
        this.kernel = kernel;
    }

    public ConfiguredValueTaskAwaitable<Result> ShowAsync<TView>(CancellationToken cancellationToken)
    {
        var view = kernel.Get<TView>();

        return this.InvokeUiBackgroundAsync(() => managedNotificationManager.Show(view));
    }

    public ConfiguredValueTaskAwaitable<Result> ShowAsync(object view, CancellationToken cancellationToken)
    {
        return this.InvokeUiBackgroundAsync(() => managedNotificationManager.Show(view));
    }
}