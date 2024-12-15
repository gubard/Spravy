using Spravy.Domain.Extensions;
using Spravy.Domain.Helpers;
using Spravy.Domain.Interfaces;

namespace Spravy.Ui.iOS.Services;

public class EmbeddedConfigurationLoader : IConfigurationLoader
{
    public Stream GetStream()
    {
        var stream = SpravyUiiOSMark.GetResourceStream(FileNames.DefaultConfigFileName);

        return stream.ThrowIfNull();
    }
}