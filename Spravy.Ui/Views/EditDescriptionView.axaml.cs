using Avalonia.ReactiveUI;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class EditDescriptionView : ReactiveUserControl<EditDescriptionViewModel>
{
    public EditDescriptionView()
    {
        InitializeComponent();
    }
}