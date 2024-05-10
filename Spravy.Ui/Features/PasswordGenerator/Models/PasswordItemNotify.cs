namespace Spravy.Ui.Features.PasswordGenerator.Models;

public class PasswordItemNotify : NotifyBase, IIdProperty
{
    public PasswordItemNotify(
        PasswordItem passwordItem,
        IPasswordService passwordService,
        IDialogViewer dialogViewer,
        IUiApplicationService uiApplicationService,
        IErrorHandler errorHandler
    )
    {
        Name = passwordItem.Name;
        
        DeletePasswordItem = SpravyCommand.CreateDeletePasswordItem(passwordItem.Id, passwordService, dialogViewer,
            uiApplicationService, errorHandler);
    }
    
    [Reactive]
    public string Name { get; set; }
    
    [Reactive]
    public Guid Id { get; set; }
    
    public SpravyCommand DeletePasswordItem { get; }
}