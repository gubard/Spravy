namespace Spravy.Ui.Converters;

public class LinesSubstringConverter : IValueConverter
{
    public ushort MaxLines { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string str)
        {
            return value;
        }

        return string.Join(
            Environment.NewLine,
            str.Split(
                    '\r',
                    '\n'
                )
               .Take(MaxLines)
        );
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}