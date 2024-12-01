namespace Spravy.Ui.Extensions;

public static class TextBoxExtension
{
    public static Result FocusTextBoxUi<T>(this T textBox) where T : TextBox
    {
        textBox.Focus();

        if (textBox.Text is null)
        {
            return Result.Success;
        }

        textBox.CaretIndex = textBox.Text.Length;

        return Result.Success;
    }
}