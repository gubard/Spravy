namespace Spravy.Ui.Features.Localizations.Converters;

public class ObjectToLocalizationConverter : IValueConverter
{
    private static readonly Application application = Application.Current.ThrowIfNull();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (parameter is not string templateKey)
        {
            return "Template key must be string.";
        }

        if (templateKey.IsNullOrWhiteSpace())
        {
            return "Template key not set.";
        }

        if (value is null)
        {
            return "Parameters not set.";
        }

        if (!application.TryGetResource(templateKey, out var resource))
        {
            return $"Can't find template with key {templateKey}.";
        }

        if (resource is not string template)
        {
            return "Template must be string.";
        }

        if (value is not IObjectParameters parameters)
        {
            return $"Parameters must be {nameof(IObjectParameters)}.";
        }

        var result = SpravyFormat.Format(template, parameters);

        if (result.TryGetValue(out var formatted))
        {
            return formatted;
        }

        return string.Join(Environment.NewLine, result.Errors.Select(x => x.Message).ToArray());
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}