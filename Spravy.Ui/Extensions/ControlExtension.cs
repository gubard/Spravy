namespace Spravy.Ui.Extensions;

public static class ControlExtension
{
    public static Result FocusTextBoxUi<T>(this T control, string textBoxName) where T : Control
    {
        return control.GetControl<TextBox>(textBoxName).FocusTextBoxUi();
    }
}