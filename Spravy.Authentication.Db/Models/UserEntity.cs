namespace Spravy.Authentication.Db.Models;

public class UserEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Login { get; set; }
    public string PasswordHash { get; set; }
    public string Salt { get; set; }
    public string HashMethod { get; set; }
}