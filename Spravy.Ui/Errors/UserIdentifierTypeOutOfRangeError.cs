namespace Spravy.Ui.Errors;

public class UserIdentifierTypeOutOfRangeError : ValueOutOfRangeError<UserIdentifierType>
{
    public static readonly Guid MainId = new("F2F33B58-F58C-47AE-982A-70C367EF8AB6");

    protected UserIdentifierTypeOutOfRangeError()
        : base(UserIdentifierType.Email, MainId) { }

    public UserIdentifierTypeOutOfRangeError(UserIdentifierType value)
        : base(value, MainId) { }
}
