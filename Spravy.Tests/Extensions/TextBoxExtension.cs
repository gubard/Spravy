using Avalonia.Controls;
using Avalonia.Input;
using Spravy.Domain.Extensions;
using Xunit.Abstractions;

namespace Spravy.Tests.Extensions;

public static class TextBoxExtension
{
    public static TTextBox ClearText<TTextBox>(this TTextBox textBox, Window window, ITestOutputHelper output)
        where TTextBox : TextBox
    {
        textBox.Text.ThrowIfNullOrEmpty();
        textBox.MustFocused();

        for (var i = 0; i < textBox.Text.ThrowIfNull().Length; i++)
        {
            window.KeyPressQwerty(PhysicalKey.ArrowRight, RawInputModifiers.None, output);
            window.RunJobsAll(output);
            window.KeyReleaseQwerty(PhysicalKey.ArrowRight, RawInputModifiers.None, output);
            window.RunJobsAll(output);
        }

        while (textBox.Text.ThrowIfNull().Length > 0)
        {
            window.KeyPressQwerty(PhysicalKey.Backspace, RawInputModifiers.None, output);
            window.RunJobsAll(output);
            window.KeyReleaseQwerty(PhysicalKey.Backspace, RawInputModifiers.None, output);
            window.RunJobsAll(output);
        }

        return textBox;
    }
}