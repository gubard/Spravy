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

    public Result Show<TView>() where TView : notnull
    {
        var view = serviceFactory.CreateService<TView>();

        return this.PostUiBackground(
            () =>
            {
                managedNotificationManager.Show(view);

                return Result.Success;
            },
            CancellationToken.None
        );
    }

    public Result Show(object view)
    {
        return this.PostUiBackground(
            () =>
            {
                managedNotificationManager.Show(view);

                return Result.Success;
            },
            CancellationToken.None
        );
    }
}