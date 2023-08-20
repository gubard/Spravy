using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace Spravy.Ui.Controls;

[TemplatePart(TextBoxEditablePartName, typeof(TextBox))]
[TemplatePart(TextBoxReadOnlyPartName, typeof(TextBox))]
[TemplatePart(EditButtonPartName, typeof(Button))]
[TemplatePart(OkButtonPartName, typeof(Button))]
[TemplatePart(CancelButtonPartName, typeof(Button))]
public class EditableTextBlock : TemplatedControl
{
    public const string TextBoxEditablePartName = "PART_EditableTextBox";
    public const string TextBoxReadOnlyPartName = "PART_ReadOnlyTextBox";
    public const string EditButtonPartName = "PART_EditButton";
    public const string OkButtonPartName = "PART_OkButton";
    public const string CancelButtonPartName = "PART_CancelButton";

    /// <summary>
    /// Defines the <see cref="Text"/> property.
    /// </summary>
    public static readonly StyledProperty<string?> TextProperty = TextBlock.TextProperty.AddOwner<EditableTextBlock>();

    /// <summary>
    /// Defines the <see cref="AcceptsReturn"/> property
    /// </summary>
    public static readonly StyledProperty<bool> AcceptsReturnProperty =
        TextBox.AcceptsReturnProperty.AddOwner<EditableTextBlock>();
    
    /// <summary>
    /// Defines the <see cref="Watermark"/> property
    /// </summary>
    public static readonly StyledProperty<string?> WatermarkProperty =
        TextBox.WatermarkProperty.AddOwner<EditableTextBlock>();

    /// <summary>
    /// Defines the <see cref="AcceptsTab"/> property
    /// </summary>
    public static readonly StyledProperty<bool> AcceptsTabProperty =
        TextBox.AcceptsTabProperty.AddOwner<EditableTextBlock>();

    public static readonly StyledProperty<TextWrapping> TextWrappingProperty =
        TextBlock.TextWrappingProperty.AddOwner<EditableTextBlock>();

    private TextBox? readOnlyTextBox;
    private TextBox? editableTextBox;
    private Button? editButton;
    private Button? okButton;
    private Button? cancelButton;

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// Gets or sets a value that determines whether the TextBox allows and displays newline or return characters
    /// </summary>
    public bool AcceptsReturn
    {
        get => GetValue(AcceptsReturnProperty);
        set => SetValue(AcceptsReturnProperty, value);
    }

    /// <summary>
    /// Gets or sets a value that determins whether the TextBox allows and displays tabs
    /// </summary>
    public bool AcceptsTab
    {
        get => GetValue(AcceptsTabProperty);
        set => SetValue(AcceptsTabProperty, value);
    }

    /// <summary>
    /// Gets or sets the <see cref="Media.TextWrapping"/> of the TextBox
    /// </summary>
    public TextWrapping TextWrapping
    {
        get => GetValue(TextWrappingProperty);
        set => SetValue(TextWrappingProperty, value);
    }
    
    /// <summary>
    /// Gets or sets the placeholder or descriptive text that is displayed even if the <see cref="Text"/>
    /// property is not yet set.
    /// </summary>
    public string? Watermark
    {
        get => GetValue(WatermarkProperty);
        set => SetValue(WatermarkProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (editButton is not null)
        {
            editButton.Click -= Edit;
        }

        if (okButton is not null)
        {
            okButton.Click -= Ok;
        }

        if (cancelButton is not null)
        {
            cancelButton.Click -= Cancel;
        }

        editableTextBox = e.NameScope.Get<TextBox>(TextBoxEditablePartName);
        readOnlyTextBox = e.NameScope.Get<TextBox>(TextBoxReadOnlyPartName);
        editButton = e.NameScope.Get<Button>(EditButtonPartName);
        okButton = e.NameScope.Get<Button>(OkButtonPartName);
        cancelButton = e.NameScope.Get<Button>(CancelButtonPartName);
        editButton.Click += Edit;
        okButton.Click += Ok;
        cancelButton.Click += Cancel;
    }

    private void Edit(object? sender, RoutedEventArgs e)
    {
        if (!IsInited())
        {
            return;
        }

        editableTextBox.Text = readOnlyTextBox.Text;
        readOnlyTextBox.IsVisible = false;
        editableTextBox.IsVisible = true;
        editButton.IsVisible = false;
        okButton.IsVisible = true;
        cancelButton.IsVisible = true;
        editableTextBox.Focus();
        editableTextBox.CaretIndex = editableTextBox.Text?.Length ?? 0;
    }

    private void Ok(object? sender, RoutedEventArgs e)
    {
        if (!IsInited())
        {
            return;
        }

        readOnlyTextBox.Text = editableTextBox.Text;
        readOnlyTextBox.IsVisible = true;
        editableTextBox.IsVisible = false;
        editButton.IsVisible = true;
        okButton.IsVisible = false;
        cancelButton.IsVisible = false;
    }

    private void Cancel(object? sender, RoutedEventArgs e)
    {
        if (!IsInited())
        {
            return;
        }

        readOnlyTextBox.IsVisible = true;
        editableTextBox.IsVisible = false;
        editButton.IsVisible = true;
        okButton.IsVisible = false;
        cancelButton.IsVisible = false;
    }

    private bool IsInited()
    {
        if (readOnlyTextBox is null)
        {
            return false;
        }

        if (editableTextBox is null)
        {
            return false;
        }

        if (editButton is null)
        {
            return false;
        }

        if (okButton is null)
        {
            return false;
        }

        if (cancelButton is null)
        {
            return false;
        }

        return true;
    }
}