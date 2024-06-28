namespace Spravy.Ui.Features.PasswordGenerator.Models;

public class PasswordItemNotify : NotifyBase, IPasswordItem, IIdProperty
{
    public PasswordItemNotify(PasswordItem passwordItem, SpravyCommandService spravyCommandService)
    {
        Name = passwordItem.Name;
        Id = passwordItem.Id;
        DeletePasswordItem = spravyCommandService.DeletePasswordItem;
        GeneratePassword = spravyCommandService.GeneratePassword;
    }

    [Reactive]
    public string Name { get; set; }

    [Reactive]
    public Guid Id { get; set; }

    public SpravyCommand DeletePasswordItem { get; private set; }
    public SpravyCommand GeneratePassword { get; private set; }
}
