namespace Spravy.Service.Model;

public class JwtOptions
{
    public const string Section = "Jwt";

    public string? Key { get; set; }
    public string? Issuer { get; set; }
    public string? Audience { get; set; }
}