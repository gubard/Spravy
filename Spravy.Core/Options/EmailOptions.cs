using Spravy.Domain.Interfaces;

namespace Spravy.Core.Options;

public class EmailOptions : IOptionsValue
{
    public static string Section => "EmailService";

    public string Host { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}