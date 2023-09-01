using Spravy.Domain.Interfaces;

namespace Spravy.Authentication.Service.Models;

public class JwtTokenFactoryOptions : IOptionsValue
{
    public static string Section => "Jwt";
    
    public string? Key { get; set; }
    public ushort ExpiresDays { get; set; }
    public string? Issuer { get; set; }
    public string? Audience { get; set; }
}