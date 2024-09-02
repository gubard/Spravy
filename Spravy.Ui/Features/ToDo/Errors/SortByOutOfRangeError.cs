namespace Spravy.Ui.Features.ToDo.Errors;

public class SortByOutOfRangeError : ValueOutOfRangeError<SortBy>
{
    public static readonly Guid MainId = new("D4EC3D6C-23C2-4736-84D5-580E92410CB1");

    protected SortByOutOfRangeError()
        : base(SortBy.OrderIndex, MainId) { }

    public SortByOutOfRangeError(SortBy value)
        : base(value, MainId) { }
}
