namespace Spravy.Ui.Features.Localizations.Models;

public class TextLocalization
{
    private static readonly Application application = Application.Current.ThrowIfNull();

    public TextLocalization(string key)
    {
        Key = key;
    }

    public TextLocalization(string key, IParameters? parameters)
    {
        Key = key;
        Parameters = parameters;
    }

    public string Key { get; }
    public IParameters? Parameters { get; }

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
            
            var result = SpravyFormat.Format(str, Parameters);

            if (result.TryGetValue(out var formatted))
            {
                return formatted;
            }

            return string.Join(Environment.NewLine, result.Errors.Select(x => x.Message).ToArray());
        }
    }
}
