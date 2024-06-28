namespace Spravy.Ui.Converters;

public class ToDoItemTypeLocalizationValueConverter : IValueConverter
{
    private readonly Application application = Application.Current.ThrowIfNull();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
        {
            return null;
        }

        if (value is not ToDoItemType type)
        {
            return null;
        }

        switch (type)
        {
            case ToDoItemType.Value:
                return application.GetResource("ToDoItemType.Value");
            case ToDoItemType.Group:
                return application.GetResource("ToDoItemType.Group");
            case ToDoItemType.Planned:
                return application.GetResource("ToDoItemType.Planned");
            case ToDoItemType.Periodicity:
                return application.GetResource("ToDoItemType.Periodicity");
            case ToDoItemType.PeriodicityOffset:
                return application.GetResource("ToDoItemType.PeriodicityOffset");
            case ToDoItemType.Circle:
                return application.GetResource("ToDoItemType.Circle");
            case ToDoItemType.Step:
                return application.GetResource("ToDoItemType.Step");
            case ToDoItemType.Reference:
                return application.GetResource("ToDoItemType.Reference");
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
