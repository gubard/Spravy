using Avalonia.ReactiveUI;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Views;

public partial class InfoView : ReactiveUserControl<InfoViewModel>
{
    public const string OkButtonName = "ok-button";

    public InfoView()
    {
        InitializeComponent();
    }
}
