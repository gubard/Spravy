using System;
using System.Globalization;
using Avalonia.Data.Converters;
using ExtensionFramework.Core.Common.Extensions;

namespace Spravy.Ui.Converters;

public class EmptyStringToNullConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string str)
        {
            if (str.IsNullOrWhiteSpace())
            {
                return null;
            }
        }

        return value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
        {
            return string.Empty;
        }

        return value;
    }
}