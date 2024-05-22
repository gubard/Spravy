namespace Spravy.Domain.Services;

public class RandomStringGuid : IRandom<string>
{
    public static readonly RandomStringGuid Digits = new(RandomGuid.Default, GuidFormats.Digits);
    public static readonly RandomStringGuid Braces = new(RandomGuid.Default, GuidFormats.Braces);
    public static readonly RandomStringGuid Hyphens = new(RandomGuid.Default, GuidFormats.Hyphens);

    public static readonly RandomStringGuid Parentheses = new(RandomGuid.Default, GuidFormats.Parentheses);

    public static readonly RandomStringGuid Hexadecimal = new(RandomGuid.Default, GuidFormats.Hexadecimal);

    private readonly string format;
    private readonly IRandom<Guid> randomGuid;

    public RandomStringGuid(IRandom<Guid> randomGuid, string format)
    {
        this.randomGuid = randomGuid;
        this.format = format.ThrowIfNullOrWhiteSpace();
    }

    public string GetRandom()
    {
        return randomGuid.GetRandom().ToString(format);
    }
}