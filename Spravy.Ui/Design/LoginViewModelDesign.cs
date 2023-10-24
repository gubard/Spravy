using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Design;

public class LoginViewModelDesign : LoginViewModel
{
    public LoginViewModelDesign()
    {
        Navigator = ConstDesign.Navigator;
        Mapper = ConstDesign.Mapper;
        DialogViewer = ConstDesign.DialogViewer;
    }
}