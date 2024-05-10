namespace Spravy.Ui.Features.PasswordGenerator.Interfaces;

public interface IPasswordItemCache
{
    PasswordItemNotify GetPasswordItem(Guid id);
    ConfiguredValueTaskAwaitable<Result> UpdateAsync(PasswordItem passwordItem);
}