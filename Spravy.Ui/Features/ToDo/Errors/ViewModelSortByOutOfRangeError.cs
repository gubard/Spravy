namespace Spravy.Ui.Features.ToDo.Errors;

public class ViewModelSortByOutOfRangeError : ValueOutOfRangeError<ViewModelSortBy>
{
    public static readonly Guid MainId = new("D4EC3D6C-23C2-4736-84D5-580E92410CB1");

    protected ViewModelSortByOutOfRangeError() : base(ViewModelSortBy.OrderIndex, MainId)
    {
    }

    public ViewModelSortByOutOfRangeError(ViewModelSortBy value) : base(value, MainId)
    {
    }
}