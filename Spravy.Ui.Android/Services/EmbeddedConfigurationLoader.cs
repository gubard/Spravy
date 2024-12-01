namespace Spravy.Ui.Android.Services;

public class EmbeddedConfigurationLoader : IConfigurationLoader
{
    public Stream GetStream()
    {
        var stream = SpravyUiAndroidMark.GetResourceStream(FileNames.DefaultConfigFileName);

        return stream.ThrowIfNull();
    }
}