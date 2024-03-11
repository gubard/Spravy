using System.IO;

namespace _build.Models;

public class AndroidProjectBuilderOptions
{
    public AndroidProjectBuilderOptions(
        FileInfo keyStoreFile,
        string androidSigningKeyPass,
        string androidSigningStorePass
    )
    {
        KeyStoreFile = keyStoreFile;
        AndroidSigningKeyPass = androidSigningKeyPass;
        AndroidSigningStorePass = androidSigningStorePass;
    }

    public FileInfo KeyStoreFile { get; }
    public string AndroidSigningKeyPass { get; }
    public string AndroidSigningStorePass { get; }
}