namespace Spravy.Ui.Converters;

public class ToDoItemStatusLocalizationValueConverter : IValueConverter
{
    private readonly Application application = Application.Current.ThrowIfNull();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
        {
            return null;
        }

        if (value is not ToDoItemStatus status)
        {
            return null;
        }

        switch (status)
        {
            case ToDoItemStatus.ReadyForComplete:
                return application.GetResource("ToDoItemStatus.ReadyForComplete");
            case ToDoItemStatus.Completed:
                return application.GetResource("ToDoItemStatus.Completed");
            case ToDoItemStatus.Planned:
                return application.GetResource("ToDoItemStatus.Planned");
            case ToDoItemStatus.Miss:
                return application.GetResource("ToDoItemStatus.Miss");
            case ToDoItemStatus.ComingSoon:
                return application.GetResource("ToDoItemStatus.ComingSoon");
            default:
                throw new ArgumentOutOfRangeException();
        }
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
