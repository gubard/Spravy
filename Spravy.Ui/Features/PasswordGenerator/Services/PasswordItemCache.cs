namespace Spravy.Ui.Features.PasswordGenerator.Services;

public class PasswordItemCache : IPasswordItemCache
{
    private readonly Dictionary<Guid, PasswordItemNotify> cache;
    private readonly IDialogViewer dialogViewer;
    private readonly IUiApplicationService uiApplicationService;
    private readonly IPasswordService passwordService;
    private readonly IClipboardService clipboardService;
    private readonly ISpravyNotificationManager spravyNotificationManager;
    private readonly IErrorHandler errorHandler;
    private readonly ITaskProgressService taskProgressService;

    public PasswordItemCache(
        IDialogViewer dialogViewer,
        IUiApplicationService uiApplicationService,
        IPasswordService passwordService,
        IClipboardService clipboardService,
        ISpravyNotificationManager spravyNotificationManager,
        IErrorHandler errorHandler,
        ITaskProgressService taskProgressService
    )
    {
        cache = new();
        this.dialogViewer = dialogViewer;
        this.uiApplicationService = uiApplicationService;
        this.passwordService = passwordService;
        this.clipboardService = clipboardService;
        this.spravyNotificationManager = spravyNotificationManager;
        this.errorHandler = errorHandler;
        this.taskProgressService = taskProgressService;
    }
    
    public PasswordItemNotify GetPasswordItem(Guid id)
    {
        if (cache.TryGetValue(id, out var value))
        {
            return value;
        }
        
        var result = new PasswordItemNotify(
            new(id, "Loading...", string.Empty, 512, string.Empty, true, true, true, true, string.Empty),
            passwordService, dialogViewer, uiApplicationService, errorHandler, clipboardService,
            spravyNotificationManager, taskProgressService);
        
        cache.Add(id, result);
        
        return result;
    }
    
    public ConfiguredValueTaskAwaitable<Result> UpdateAsync(PasswordItem passwordItem)
    {
        var item = GetPasswordItem(passwordItem.Id);
        
        return this.InvokeUiBackgroundAsync(() =>
        {
             item.Name = passwordItem.Name;
             
             return Result.Success;
        });
    }
}