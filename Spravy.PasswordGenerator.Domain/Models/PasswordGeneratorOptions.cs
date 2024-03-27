using Spravy.Domain.Interfaces;

namespace Spravy.PasswordGenerator.Domain.Models;

public class PasswordGeneratorOptions : IOptionsValue
{
    public static string Section => "PasswordGenerator";

    public byte TryCount { get; set; }
}