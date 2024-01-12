using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Spravy.Domain.Extensions;

namespace Spravy.Tests.Extensions;

public static class TextBoxExtension
{
    public static TTextBox ClearText<TTextBox>(this TTextBox textBox, Window window) where TTextBox : TextBox
    {
        textBox.Text.ThrowIfNullOrEmpty();
        textBox.Text = string.Empty;
        window.RunJobsAll();
        /*textBox.MustFocused();

        for (var i = 0; i < textBox.Text.ThrowIfNull().Length; i++)
        {
            window.KeyPressQwerty(PhysicalKey.ArrowRight, RawInputModifiers.None);
            window.RunJobsAll();
            window.KeyReleaseQwerty(PhysicalKey.ArrowRight, RawInputModifiers.None);
            window.RunJobsAll();
        }

        if (!textBox.IsFocused)
        {
            textBox.FocusElement();
        }

        while (textBox.Text.ThrowIfNull().Length > 0)
        {
            window.KeyPressQwerty(PhysicalKey.Backspace, RawInputModifiers.None);
            window.RunJobsAll();
            window.KeyReleaseQwerty(PhysicalKey.Backspace, RawInputModifiers.None);
            window.RunJobsAll();
        }*/

        return textBox;
    }
}