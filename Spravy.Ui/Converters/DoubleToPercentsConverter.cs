using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Spravy.Ui.Converters;

public class DoubleToPercentsConverter : IValueConverter
{
    public double Percentage { get; set; }

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not double number)
        {
            return value;
        }

        return number * Percentage;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not double number)
        {
            return value;
        }

        return number / Percentage;
    }
}