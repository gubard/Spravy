namespace Spravy.Ui.Controls;

public class CopiedTextControl : TemplatedControl
{
    public static readonly StyledProperty<string?> TextProperty =
        TextBox.TextProperty.AddOwner<CopiedTextControl>();

    private Button? buttonCopy;

    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        var button = e.NameScope.Find<Button>("PART_ButtonCopy");
        UpdateButtonCopy(button);
    }

    private void UpdateButtonCopy(Button? button)
    {
        if (buttonCopy is not null)
        {
            buttonCopy.Click -= OnButtonCopyClick;
        }

        buttonCopy = button;

        if (buttonCopy is null)
        {
            return;
        }

        buttonCopy.Click += OnButtonCopyClick;
    }

    private async void OnButtonCopyClick(object? sender, RoutedEventArgs args)
    {
        if (Text is null)
        {
            return;
        }

        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;

        if (clipboard is null)
        {
            return;
        }

        await clipboard.SetTextAsync(Text);
    }
}
