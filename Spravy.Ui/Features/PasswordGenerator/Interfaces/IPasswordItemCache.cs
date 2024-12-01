namespace Spravy.Ui.Features.PasswordGenerator.Interfaces;

public interface IPasswordItemCache
{
    Result<PasswordItemEntityNotify> GetPasswordItem(Guid id);
    Result UpdateUi(PasswordItem passwordItem);
}