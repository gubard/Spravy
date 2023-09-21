using Spravy.Domain.Interfaces;

namespace Spravy.Authentication.Service.Models;

public class AuthenticationSeedServiceOptions : IOptionsValue
{
    public static string Section => "AuthenticationSeedService";

    public string? DataFilePath { get; set; }
}