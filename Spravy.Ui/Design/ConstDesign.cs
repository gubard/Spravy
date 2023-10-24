using AutoMapper;
using Spravy.Ui.Configurations;
using Spravy.Ui.Interfaces;

namespace Spravy.Ui.Design;

public class ConstDesign
{
    public static readonly IMapper Mapper = new Mapper(new MapperConfiguration(UiModule.SetupMapperConfiguration));
    public static readonly IDialogViewer DialogViewer = new DialogViewerDesign();
    public static readonly INavigator Navigator = new NavigatorDesign();
}