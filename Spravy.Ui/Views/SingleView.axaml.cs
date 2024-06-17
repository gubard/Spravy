namespace Spravy.Ui.Views;

public partial class SingleView : UserControl, ISingleViewTopLevelControl
{
    public SingleView(MainViewModel viewModel)
    {
        InitializeComponent();
        Content = viewModel;
    }
}