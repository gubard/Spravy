namespace Spravy.Ui.Helpers;

public static class LanguageHelper
{
    public static ReadOnlyMemory<string> Languages = new[]
    {
        "en-US",
        "uk-UA",
    };

    public static string DefaultLanguage;

    static LanguageHelper()
    {
        var name = CultureInfo.CurrentCulture.Name;
        DefaultLanguage = Languages.Contains(name) ? name : Languages.Span[0];
    }
}