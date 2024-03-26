using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Spravy.Ui.Features.ToDo.ViewModels;

namespace Spravy.Ui.Features.ToDo.Views;

[TemplatePart(NameTextBoxName, typeof(TextBox))]
public partial class AddRootToDoItemView : ReactiveUserControl<AddRootToDoItemViewModel>
{
    public const string NameTextBoxName = "NameTextBox";

    public AddRootToDoItemView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        this.FindControl<TextBox>(NameTextBoxName)?.Focus();
    }
}