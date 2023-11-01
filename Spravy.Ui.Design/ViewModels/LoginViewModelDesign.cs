using Spravy.Ui.Design.Helpers;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Design.ViewModels;

public class LoginViewModelDesign : LoginViewModel
{
    public LoginViewModelDesign()
    {
        Navigator = ConstDesign.Navigator;
        Mapper = ConstDesign.Mapper;
        DialogViewer = ConstDesign.DialogViewer;
    }
}