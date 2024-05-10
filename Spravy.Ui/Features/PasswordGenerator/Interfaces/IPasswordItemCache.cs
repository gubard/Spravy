namespace Spravy.Ui.Features.PasswordGenerator.Interfaces;

public interface IPasswordItemCache
{
    PasswordItemNotify GetPasswordItem(Guid id);
    void UpdatePasswordItem(PasswordItem passwordItem);
}