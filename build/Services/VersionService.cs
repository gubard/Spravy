using System.IO;
using _build.Models;

namespace _build.Services;

public class VersionService
{
    readonly FileInfo fileVersion;
    SpravyVersion version;

    public VersionService(FileInfo fileVersion)
    {
        this.fileVersion = fileVersion;
    }

    public SpravyVersion Version => version;

    public void Load()
    {
        LoadVersion();
        version++;
    }
    
    public void Save()
    {
        UpdateVersion();
    }

    void LoadVersion()
    {
        if (fileVersion.Exists)
        {
            SpravyVersion.TryParse(File.ReadAllText(fileVersion.FullName), out version);

            return;
        }

        version = new(1, 0, 0, 0);
    }

    void UpdateVersion()
    {
        if (fileVersion.Directory is not null)
        {
            if (!fileVersion.Directory.Exists)
            {
                fileVersion.Directory.Create();
            }
        }

        if (!fileVersion.Exists)
        {
            using var stream = fileVersion.Create();
        }

        File.WriteAllText(fileVersion.FullName, Version.ToString());
    }
}