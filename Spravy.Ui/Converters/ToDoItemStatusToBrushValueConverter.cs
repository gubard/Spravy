using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Spravy.ToDo.Domain.Enums;

namespace Spravy.Ui.Converters;

public class ToDoItemStatusToBrushValueConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not ToDoItemStatus status)
        {
            return new SolidColorBrush(Colors.DarkRed);
        }

        return status switch
        {
            ToDoItemStatus.Miss => new SolidColorBrush(Colors.DarkRed),
            ToDoItemStatus.Completed => new SolidColorBrush(Colors.DarkGreen),
            ToDoItemStatus.Planned => new SolidColorBrush(Colors.PowderBlue),
            ToDoItemStatus.ReadyForComplete => new SolidColorBrush(Colors.GreenYellow),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not SolidColorBrush brush)
        {
            return ToDoItemStatus.Miss;
        }

        if (brush.Color == Colors.DarkRed)
        {
            return ToDoItemStatus.Miss;
        }

        if (brush.Color == Colors.DarkGreen)
        {
            return ToDoItemStatus.Completed;
        }

        if (brush.Color == Colors.GreenYellow)
        {
            return ToDoItemStatus.ReadyForComplete;
        }
        
        if (brush.Color == Colors.PowderBlue)
        {
            return ToDoItemStatus.Planned;
        }

        throw new ArgumentOutOfRangeException();
    }
}