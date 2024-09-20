namespace Spravy.Ui.Features.ToDo.Views;

public partial class EditDescriptionContentView : MainUserControl<EditDescriptionContentViewModel>
{
    public EditDescriptionContentView()
    {
        InitializeComponent();
        DefaultFocusTextBox = DescriptionTextBox;
    }
}
