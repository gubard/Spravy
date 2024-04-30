using Avalonia;
using Spravy.Ui.Services;

namespace Spravy.Ui.Extensions;

public static class AppBuilderExtension
{
    public static AppBuilder WithShantellSansFont(this AppBuilder appBuilder)
    {
        return appBuilder.ConfigureFonts(fontManager =>
            fontManager.AddFontCollection(new ShantellSansFontCollection()));
    }
}