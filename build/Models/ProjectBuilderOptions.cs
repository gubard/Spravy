using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using _build.Interfaces;

namespace _build.Models;

public class ProjectBuilderOptions : ICsprojFile
{
    public ProjectBuilderOptions(
        FileInfo csprojFile,
        FileInfo appSettingsFile,
        IReadOnlyDictionary<string, ushort> hosts,
        IEnumerable<Runtime> runtimes,
        string configuration,
        string domain
    )
    {
        CsprojFile = csprojFile;
        AppSettingsFile = appSettingsFile;
        Hosts = hosts;
        Runtimes = runtimes.ToArray();
        Configuration = configuration;
        Domain = domain;
    }

    public FileInfo CsprojFile { get; }
    public FileInfo AppSettingsFile { get; }
    public IReadOnlyDictionary<string, ushort> Hosts { get; }
    public ReadOnlyMemory<Runtime> Runtimes { get; }
    public string Configuration { get; }
    public string Domain { get; }
}