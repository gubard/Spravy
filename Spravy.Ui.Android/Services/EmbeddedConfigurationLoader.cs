using Microsoft.Extensions.Configuration;
using Spravy.Domain.Extensions;
using Spravy.Domain.Helpers;
using Spravy.Domain.Interfaces;
using Spravy.Ui.Interfaces;

namespace Spravy.Ui.Android.Services;

public class EmbeddedConfigurationLoader : IConfigurationLoader
{
    public Stream GetStream()
    {
        var stream = SpravyUiAndroidMark.GetResourceStream(FileNames.DefaultConfigFileName);

        return stream.ThrowIfNull();
    }
}
