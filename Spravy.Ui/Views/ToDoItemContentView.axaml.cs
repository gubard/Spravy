using Avalonia.ReactiveUI;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class ToDoItemContentView : ReactiveUserControl<ToDoItemContentViewModel>
{
    public const string NameTextBoxName = "name-text-box";

    public ToDoItemContentView()
    {
        InitializeComponent();
    }
}