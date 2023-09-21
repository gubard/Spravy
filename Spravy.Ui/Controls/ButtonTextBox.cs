using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Styling;
using Material.Icons;
using Material.Icons.Avalonia;

namespace Spravy.Ui.Controls;

[TemplatePart(ButtonName, typeof(Button))]
[TemplatePart(MaterialIconName, typeof(MaterialIcon))]
[TemplatePart(MaterialIconName, typeof(TextBox))]
public class ButtonTextBox : TemplatedControl
{
    public const string ButtonName = "PART_Button";
    public const string MaterialIconName = "PART_MaterialIcon";
    public const string TextBoxName = "PART_TextBox";

    private Button? button;
    private MaterialIcon? materialIcon;

    public static readonly StyledProperty<ICommand?> CommandProperty =
        Button.CommandProperty.AddOwner<ButtonTextBox>();

    public static readonly StyledProperty<object?> CommandParameterProperty =
        Button.CommandParameterProperty.AddOwner<ButtonTextBox>();

    public static readonly StyledProperty<string?> TextProperty =
        TextBox.TextProperty.AddOwner<ButtonTextBox>();

    public static readonly StyledProperty<string?> WatermarkProperty =
        TextBox.WatermarkProperty.AddOwner<ButtonTextBox>();

    public static readonly AttachedProperty<string?> LabelProperty =
        AvaloniaProperty.RegisterAttached<ButtonTextBox, string?>(nameof(Label), typeof(ButtonTextBox));

    public static readonly StyledProperty<MaterialIconKind> KindProperty =
        MaterialIcon.KindProperty.AddOwner<ButtonTextBox>();

    public static readonly StyledProperty<ControlTheme?> TextBoxThemeProperty =
        AvaloniaProperty.Register<ButtonTextBox, ControlTheme?>(nameof(TextBoxTheme));

    public static readonly StyledProperty<bool> IsReadOnlyProperty =
        TextBox.IsReadOnlyProperty.AddOwner<ButtonTextBox>();

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        button = e.NameScope.Find<Button>(ButtonName);
        materialIcon = e.NameScope.Find<MaterialIcon>(MaterialIconName);

        if (button is not null)
        {
            button.Command = Command;
            button.CommandParameter = CommandParameter;
        }

        if (materialIcon is not null)
        {
            materialIcon.Kind = Kind;
        }
    }

    public bool IsReadOnly
    {
        get => GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }

    public ControlTheme? TextBoxTheme
    {
        get => GetValue(TextBoxThemeProperty);
        set => SetValue(TextBoxThemeProperty, value);
    }

    public ICommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public string? Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public string? Watermark
    {
        get => GetValue(WatermarkProperty);
        set => SetValue(WatermarkProperty, value);
    }

    public MaterialIconKind Kind
    {
        get => GetValue(KindProperty);
        set => SetValue(KindProperty, value);
    }
}