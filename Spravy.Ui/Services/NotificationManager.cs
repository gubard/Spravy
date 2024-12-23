namespace Spravy.Ui.Services;

public class NotificationManager : ISpravyNotificationManager
{
    private readonly IManagedNotificationManager managedNotificationManager;
    private readonly IServiceFactory serviceFactory;

    public NotificationManager(IManagedNotificationManager managedNotificationManager, IServiceFactory serviceFactory)
    {
        this.managedNotificationManager = managedNotificationManager;
        this.serviceFactory = serviceFactory;
    }

    public Cvtar ShowAsync<TView>(CancellationToken ct) where TView : notnull
    {
        var view = serviceFactory.CreateService<TView>();

        return this.InvokeUiBackgroundAsync(
            () =>
            {
                managedNotificationManager.Show(view);

                return Result.Success;
            }
        );
    }

    public Cvtar ShowAsync(object view, CancellationToken ct)
    {
        return this.InvokeUiBackgroundAsync(
            () =>
            {
                managedNotificationManager.Show(view);

                return Result.Success;
            }
        );
    }
}