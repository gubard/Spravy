namespace Spravy.Domain.Errors;

public class RoleOutOfRangeError : ValueOutOfRangeError<Role>
{
    public static readonly Guid MainId = new("7AA58F11-9755-4F49-9478-85EA1778EA9D");

    protected RoleOutOfRangeError() : base(Role.User, MainId)
    {
    }

    public RoleOutOfRangeError(Role value) : base(value, MainId)
    {
    }
}