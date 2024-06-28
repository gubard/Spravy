namespace Spravy.Core.Options;

public class EmailOptions : IOptionsValue
{
    public string Host { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public static string Section
    {
        get => "EmailService";
    }
}
