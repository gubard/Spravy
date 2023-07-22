using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Spravy.Ui.Models;

namespace Spravy.Ui.Converters;

public class ToDoItemNotifyToIsVisibleCompletedCountConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not ToDoSubItemValueNotify item)
        {
            return false;
        }

        return item.CompletedCount > 0;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}