namespace Spravy.Ui.Services;

public class NotificationManager : ISpravyNotificationManager
{
    private readonly IServiceFactory serviceFactory;
    private readonly IManagedNotificationManager managedNotificationManager;
    
    public NotificationManager(IManagedNotificationManager managedNotificationManager, IServiceFactory serviceFactory)
    {
        this.managedNotificationManager = managedNotificationManager;
        this.serviceFactory = serviceFactory;
    }
    
    public ConfiguredValueTaskAwaitable<Result> ShowAsync<TView>(CancellationToken cancellationToken)
        where TView : notnull
    {
        var view = serviceFactory.CreateService<TView>();
        
        return this.InvokeUiBackgroundAsync(() =>
        {
            managedNotificationManager.Show(view);
            
            return Result.Success;
        });
    }
    
    public ConfiguredValueTaskAwaitable<Result> ShowAsync(object view, CancellationToken cancellationToken)
    {
        return this.InvokeUiBackgroundAsync(() =>
        {
            managedNotificationManager.Show(view);
            
            return Result.Success;
        });
    }
}