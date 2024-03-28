using Avalonia;
using SmartFormat;
using Spravy.Domain.Extensions;

namespace Spravy.Ui.Features.Localizations.Models;

public class TextView
{
    private static readonly Application application = Application.Current.ThrowIfNull();

    public const string DeleteToDoItemView_QuestionKey = "DeleteToDoItemView.Question";

    public TextView(string key)
    {
        Key = key;
    }

    public TextView(string key, object? parameters)
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

            return Smart.Format(str, Parameters);
        }
    }
}