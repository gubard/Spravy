using Spravy.Ui.Features.PasswordGenerator.Interfaces;

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
    
    public PasswordItemCache(
        IDialogViewer dialogViewer,
        IUiApplicationService uiApplicationService,
        IPasswordService passwordService,
        IClipboardService clipboardService,
        ISpravyNotificationManager spravyNotificationManager,
        IErrorHandler errorHandler
    )
    {
        cache = new();
        this.dialogViewer = dialogViewer;
        this.uiApplicationService = uiApplicationService;
        this.passwordService = passwordService;
        this.clipboardService = clipboardService;
        this.spravyNotificationManager = spravyNotificationManager;
        this.errorHandler = errorHandler;
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
            spravyNotificationManager);
        
        cache.Add(id, result);
        
        return result;
    }
    
    public ConfiguredValueTaskAwaitable<Result> UpdateAsync(PasswordItem passwordItem)
    {
        var item = GetPasswordItem(passwordItem.Id);
        
        return this.InvokeUIBackgroundAsync(() => item.Name = passwordItem.Name);
    }
}