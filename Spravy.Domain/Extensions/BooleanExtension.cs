namespace Spravy.Domain.Extensions;

public static class BooleanExtension
{
    public static string IfTrueElseEmpty(this bool value, string str)
    {
        return value ? str : string.Empty;
    }
}
