using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Spravy.ToDo.Domain.Enums;

namespace Spravy.Ui.Converters;

public class ToDoItemCanCompleteToBooleanConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not ToDoItemIsCan isCan)
        {
            return false;
        }

        return isCan switch
        {
            ToDoItemIsCan.CanComplete => false,
            _ => true,
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}