namespace Spravy.Ui.Features.PasswordGenerator.Models;

public class PasswordItemEntityNotify : NotifyBase, IPasswordItem, IIdProperty
{
    public PasswordItemEntityNotify()
    {
        Name = "Loading...";
    }
    
    [Reactive]
    public string Name { get; set; }

    [Reactive]
    public Guid Id { get; set; }
}
