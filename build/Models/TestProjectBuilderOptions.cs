using System.Collections.Generic;
using System.IO;

namespace _build.Models;

public class TestProjectBuilderOptions : ProjectBuilderOptions
{

    public TestProjectBuilderOptions(
        FileInfo csprojFile,
        FileInfo appSettingsFile,
        IReadOnlyDictionary<string, ushort> hosts,
        IEnumerable<Runtime> runtimes,
        string configuration,
        string domain,
        string emailAccountPassword,
        string emailAccount2Password)
        : base(
            csprojFile,
            appSettingsFile,
            hosts,
            runtimes,
            configuration,
            domain
        )
    {
        EmailAccountPassword = emailAccountPassword;
        EmailAccount2Password = emailAccount2Password;
    }
    
    public string EmailAccountPassword { get; }
    public string EmailAccount2Password { get; }
}