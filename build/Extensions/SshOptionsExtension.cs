using System.Collections.Generic;
using Renci.SshNet;
using Serilog;
using _build.Interfaces;

namespace _build.Extensions;

public static class SshOptionsExtension
{
    public static SshClient CreateSshClient(this ISshOptions options)
    {
        return new(CreateSshConnection(options.SshHost, options.SshUser, options.SshPassword));
    }

    static ConnectionInfo CreateSshConnection(string sshHost, string sshUser, string sshPassword)
    {
        Log.Logger.Information("Connecting SSH {FtpHost} {FtpUser}", sshHost, sshUser);
        var methods = new List<AuthenticationMethod>();
        var values = sshHost.Split(":");
        var password = new PasswordAuthenticationMethod(sshUser, sshPassword);
        methods.Add(password);

        if (values.Length == 2)
        {
            return new(values[0], int.Parse(values[1]), sshUser, methods.ToArray());
        }

        return new(values[0], sshUser, methods.ToArray());
    }
}
