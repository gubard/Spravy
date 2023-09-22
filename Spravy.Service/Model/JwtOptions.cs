using Spravy.Domain.Interfaces;

namespace Spravy.Service.Model;

public class JwtOptions : IOptionsValue
{
    public static string Section => "Jwt";

    public string? Key { get; set; }
    public string? Issuer { get; set; }
    public string? Audience { get; set; }
}