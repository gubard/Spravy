using Avalonia.ReactiveUI;
using Spravy.Ui.Features.PasswordGenerator.ViewModels;

namespace Spravy.Ui.Features.PasswordGenerator.Views;

public partial class DeletePasswordItemView : ReactiveUserControl<DeletePasswordItemViewModel>
{
    public DeletePasswordItemView()
    {
        InitializeComponent();
    }
}