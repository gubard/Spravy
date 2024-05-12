namespace Spravy.Integration.Tests.Extensions;

public static class TextBoxExtension
{
    public static TTextBox ClearText<TTextBox>(this TTextBox textBox, Window window) where TTextBox : TextBox
    {
        if (textBox.Text.IsNullOrWhiteSpace())
        {
            return textBox;
        }

        textBox.FocusInput(window);

        for (var i = 0; i < textBox.Text.ThrowIfNull().Length; i++)
        {
            window.SpravyKeyPressQwerty(PhysicalKey.ArrowRight, RawInputModifiers.None);
            window.RunJobsAll();
            window.SpravyKeyReleaseQwerty(PhysicalKey.ArrowRight, RawInputModifiers.None);
            window.RunJobsAll();
        }

        CycleHelper.While(() => textBox.Text.ThrowIfNull().Length > 0, () =>
            {
                window.SpravyKeyPressQwerty(PhysicalKey.Backspace, RawInputModifiers.None);
                window.RunJobsAll();
                window.SpravyKeyReleaseQwerty(PhysicalKey.Backspace, RawInputModifiers.None);
                window.RunJobsAll();
            })
           .ThrowIfError();

        return textBox;
    }

    public static TTextBox SetText<TTextBox>(this TTextBox textBox, Window window, string text) where TTextBox : TextBox
    {
        textBox.FocusInput(window);
        window.SetKeyTextInput(text);

        return textBox;
    }

    public static TTextBox FocusInput<TTextBox>(this TTextBox textBox, Window window) where TTextBox : TextBox
    {
        textBox.ClickOn(window).RunJobsAll(2).MustFocused();

        return textBox;
    }
}