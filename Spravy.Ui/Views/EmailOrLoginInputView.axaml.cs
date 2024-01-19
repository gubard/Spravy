using Avalonia.ReactiveUI;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class EmailOrLoginInputView : ReactiveUserControl<EmailOrLoginInputViewModel>
{
    public EmailOrLoginInputView()
    {
        InitializeComponent();
    }
}