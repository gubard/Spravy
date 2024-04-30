using Spravy.Domain.Interfaces;

namespace Spravy.Domain.Services;

public class LoginValidator : Validator<string>, ILoginValidator
{
    public static readonly LoginValidator Default = new(new IValidationRule<string>[]
    {
        new SourceNotNullValidationRule<string>(),
        new StringMaxLengthValidationRule(256),
        new StringMinLengthValidationRule(4),
        new ValidCharsValidationRule("QAZWSXEDCRFVTGBYHNUJMIKOP_-=+<>|0123456789qazwsxedcrfvtgbyhnujmikolp."
           .AsMemory()),
    });

    public LoginValidator(ReadOnlyMemory<IValidationRule<string>> rules) : base(rules)
    {
    }
}