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
        ISpravyNotificationManager spravyNotificationManager,
        ITaskProgressService taskProgressService
    )
    {
        Name = passwordItem.Name;
        Id = passwordItem.Id;
        
        DeletePasswordItem = SpravyCommand.CreateDeletePasswordItem(Id, passwordService, dialogViewer,
            uiApplicationService, errorHandler, taskProgressService);
        
        GeneratePassword = SpravyCommand.CreateGeneratePassword(this, passwordService, clipboardService,
            spravyNotificationManager, errorHandler, taskProgressService);
    }
    
    [Reactive]
    public string Name { get; set; }
    
    [Reactive]
    public Guid Id { get; set; }
    
    public SpravyCommand DeletePasswordItem { get; private set; }
    public SpravyCommand GeneratePassword { get; private set; }
}