namespace Spravy.Ui.Errors;

public class DialogViewLayerOutOfRangeError : ValueOutOfRangeError<DialogViewLayer>
{
    public static readonly Guid MainId = new("30985255-1004-478E-A6C8-B96FD99AC5F9");

    protected DialogViewLayerOutOfRangeError()
        : base(DialogViewLayer.Error, MainId) { }

    public DialogViewLayerOutOfRangeError(DialogViewLayer value)
        : base(value, MainId) { }
}