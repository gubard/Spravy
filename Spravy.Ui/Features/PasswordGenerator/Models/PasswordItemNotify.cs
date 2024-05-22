namespace Spravy.Ui.Features.PasswordGenerator.Models;

public class PasswordItemNotify : NotifyBase, IPasswordItem, IIdProperty
{
    public PasswordItemNotify(
        PasswordItem passwordItem,
        IPasswordService passwordService,
        IDialogViewer dialogViewer,
        IUiApplicationService uiApplicationService,
        IErrorHandler errorHandler,
        IClipboardService clipboardService,
        ISpravyNotificationManager spravyNotificationManager
    )
    {
        Name = passwordItem.Name;
        Id = passwordItem.Id;
        
        DeletePasswordItem = SpravyCommand.CreateDeletePasswordItem(Id, passwordService, dialogViewer,
            uiApplicationService, errorHandler);
        
        GeneratePassword = SpravyCommand.CreateGeneratePassword(this, passwordService, clipboardService,
            spravyNotificationManager, errorHandler);
    }
    
    [Reactive]
    public string Name { get; set; }
    
    [Reactive]
    public Guid Id { get; set; }
    
    public SpravyCommand DeletePasswordItem { get; private set; }
    public SpravyCommand GeneratePassword { get; private set; }
}