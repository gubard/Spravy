using Avalonia.ReactiveUI;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class ChangeToDoItemOrderIndexView : ReactiveUserControl<ChangeToDoItemOrderIndexViewModel>
{
    public const string ItemsListBoxName = "items-list-box";

    public ChangeToDoItemOrderIndexView()
    {
        InitializeComponent();
    }
}