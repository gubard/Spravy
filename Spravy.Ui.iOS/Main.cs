using Spravy.Core.Helpers;
using Spravy.Ui.iOS.Modules;

namespace Spravy.Ui.iOS;

public class Application
{
    // This is the main entry point of the application.
    static void Main(string[] args)
    {
        DiHelper.ServiceFactory = new iOSServiceProvider();
        // if you want to use a different Application Delegate class from "AppDelegate"
        // you can specify it here.
        UIApplication.Main(args, null, typeof(AppDelegate));
    }
}