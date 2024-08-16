using Spravy.Domain.Extensions;
using Spravy.Domain.Helpers;
using Spravy.Domain.Interfaces;

namespace Spravy.Ui.Android.Services;

public class EmbeddedConfigurationLoader : IConfigurationLoader
{
    public Stream GetStream()
    {
        var stream = SpravyUiAndroidMark.GetResourceStream(FileNames.DefaultConfigFileName);

        return stream.ThrowIfNull();
    }
}
