namespace Spravy.Ui.Features.ToDo.Errors;

public class SortByToDoItemOutOfRangeError : ValueOutOfRangeError<SortByToDoItem>
{
    public static readonly Guid MainId = new("23EA9E48-2C80-4681-BC59-C321859CB71C");

    protected SortByToDoItemOutOfRangeError() : base(SortByToDoItem.Index, MainId)
    {
    }

    public SortByToDoItemOutOfRangeError(SortByToDoItem value) : base(value, MainId)
    {
    }
}