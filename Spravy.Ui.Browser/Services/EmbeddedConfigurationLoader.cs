using Spravy.Domain.Extensions;
using Spravy.Domain.Helpers;
using Spravy.Domain.Interfaces;

namespace Spravy.Ui.Browser.Services;

public class EmbeddedConfigurationLoader : IConfigurationLoader
{
    public Stream GetStream()
    {
        var stream = SpravyUiBrowserMark.GetResourceStream(FileNames.DefaultConfigFileName);

        return stream.ThrowIfNull();
    }
}
