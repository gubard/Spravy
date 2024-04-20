using Spravy.Domain.Interfaces;

namespace Spravy.Domain.Services;

public class PasswordValidator : Validator<string>, IPasswordValidator
{
    public static readonly PasswordValidator Default = new(
        new IValidationRule<string>[]
        {
            new SourceNotNullValidationRule<string>(),
            new StringMaxLengthValidationRule(512),
            new StringMinLengthValidationRule(8),
            new ValidCharsValidationRule(
                "QAZWSXEDCRFVTGBYHNUJMIKOP_-=+<>|0123456789qazwsxedcrfvtgbyhnujmikolp.".AsMemory()
            ),
        }
    );

    public PasswordValidator(ReadOnlyMemory<IValidationRule<string>> rules) : base(rules)
    {
    }
}