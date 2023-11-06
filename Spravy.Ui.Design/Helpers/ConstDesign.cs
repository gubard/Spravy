using AutoMapper;
using Spravy.Ui.Configurations;
using Spravy.Ui.Design.Services;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Services;

namespace Spravy.Ui.Design.Helpers;

public class ConstDesign
{
    public static readonly IMapper Mapper = new Mapper(new MapperConfiguration(UiModule.SetupMapperConfiguration));
    public static readonly IDialogViewer DialogViewer = new DialogViewerDesign();
    public static readonly INavigator Navigator = new NavigatorDesign();
    public static readonly IOpenerLink OpenerLink = new OpenerLinkDesign();
}