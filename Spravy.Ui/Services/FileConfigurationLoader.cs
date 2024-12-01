namespace Spravy.Ui.Services;

public class FileConfigurationLoader : IConfigurationLoader
{
    public Stream GetStream()
    {
        var file = new FileInfo("appsettings.json");
        var stream = file.OpenRead();

        return stream;
    }
}