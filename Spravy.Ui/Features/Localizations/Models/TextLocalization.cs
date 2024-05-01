namespace Spravy.Ui.Features.Localizations.Models;

public class TextLocalization
{
    private static readonly Application application = Application.Current.ThrowIfNull();

    public TextLocalization(string key)
    {
        Key = key;
    }

    public TextLocalization(string key, object? parameters)
    {
        Key = key;
        Parameters = parameters;
    }

    public string Key { get; }
    public object? Parameters { get; }

    public string Text
    {
        get
        {
            if (!application.TryGetResource(Key, null, out var value))
            {
                return $"Not found {Key}";
            }

            if (value is not string str)
            {
                return $"{value} is not string";
            }

            if (Parameters is null)
            {
                return str;
            }

            var result = Smart.Format(str, Parameters);

            return result;
        }
    }
}