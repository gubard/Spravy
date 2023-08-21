namespace Spravy.Authentication.Service.Models;

public class JwtTokenFactoryOptions
{
    public string Key { get; set; }
    public ushort ExpiresDays { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
}