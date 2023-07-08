using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Spravy.ViewModels;

namespace Spravy.Views;

public partial class AddToDoItemView : ReactiveUserControl<AddToDoItemViewModel>
{
    public const string NameTextBoxName = "NameTextBox";

    public AddToDoItemView()
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