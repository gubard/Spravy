namespace Spravy.PasswordGenerator.Domain.Models;

public class PasswordGeneratorOptions : IOptionsValue
{
    public byte TryCount { get; set; }

    public static string Section
    {
        get => "PasswordGenerator";
    }
}
