namespace Spravy.Ui.Converters;

public class ObjectToBooleanConverter<TObject> : IValueConverter
{
    public TObject? Value { get; set; }
    public bool Reverse { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
        {
            return false;
        }

        if (value is not TObject item)
        {
            return false;
        }

        if (Reverse)
        {
            return !Value?.Equals(item);
        }

        return Value?.Equals(item);
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        throw new NotSupportedException();
    }
}
