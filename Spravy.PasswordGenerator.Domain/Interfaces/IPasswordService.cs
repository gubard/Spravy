using Spravy.PasswordGenerator.Domain.Models;

namespace Spravy.PasswordGenerator.Domain.Interfaces;

public interface IPasswordService
{
    Task AddPasswordItemAsync(AddPasswordOptions options, CancellationToken cancellationToken);
    Task<IEnumerable<PasswordItem>> GetPasswordItemsAsync(CancellationToken cancellationToken);
    Task<PasswordItem> GetPasswordItemAsync(Guid id, CancellationToken cancellationToken);
    Task RemovePasswordItemAsync(Guid id, CancellationToken cancellationToken);
}