namespace Spravy.Service.Model;

public class JwtOptions : IOptionsValue
{
    public string? Key { get; set; }
    public string? Issuer { get; set; }
    public string? Audience { get; set; }

    public static string Section
    {
        get => "Jwt";
    }
}