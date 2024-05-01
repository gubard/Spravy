namespace Spravy.Ui.Converters;

public class ObjectToBooleanConverter<TObject> : IValueConverter
{
    public TObject? Value { get; set; }
    public bool Reverse { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        try
        {
            var item = (TObject)value;

            if (Reverse)
            {
                return !Value?.Equals(item);
            }

            return Value?.Equals(item);
        }
        catch
        {
            return false;
        }
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}