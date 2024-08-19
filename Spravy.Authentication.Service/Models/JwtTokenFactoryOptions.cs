namespace Spravy.Authentication.Service.Models;

public class JwtTokenFactoryOptions : IOptionsValue
{
    public string? Key { get; set; }
    public ushort ExpiresDays { get; set; }
    public ushort RefreshExpiresDays { get; set; }
    public string? Issuer { get; set; }
    public string? Audience { get; set; }

    public static string Section
    {
        get => "Jwt";
    }
}
