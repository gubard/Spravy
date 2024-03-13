using _build.Interfaces;
using Renci.SshNet;
using Serilog;

namespace _build.Extensions;

public static class SshOptionsExtension
{
    public static SshClient CreateSshClient(this ISshOptions options)
    {
        return new SshClient(CreateSshConnection(options.SshHost, options.SshUser, options.SshPassword));
    }

    static ConnectionInfo CreateSshConnection(string sshHost, string sshUser, string sshPassword)
    {
        Log.Logger.Information("Connecting SSH {FtpHost} {FtpUser}", sshHost, sshUser);
        var values = sshHost.Split(":");
        var password = new PasswordAuthenticationMethod(sshUser, sshPassword);

        if (values.Length == 2)
        {
            return new ConnectionInfo(values[0], int.Parse(values[1]), sshUser, password);
        }

        return new ConnectionInfo(values[0], sshUser, password);
    }
}