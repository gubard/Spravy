using System;
using _build.Interfaces;
using Renci.SshNet;
using Serilog;

namespace _build.Extensions;

public static class SshClientExtension
{
    public static void SafeRun(this SshClient client, string command, Func<string, bool> ignoreError = null)
    {
        using var runCommand = client.RunCommand(command);
        Log.Logger.Information("Run SSH command: {Command}", command);

        if (ignoreError is null)
        {
            if (!string.IsNullOrWhiteSpace(runCommand.Error) && !runCommand.Error.StartsWith("[sudo] password for"))
            {
                throw new(runCommand.Error);
            }
        }
        else
        {
            if (!ignoreError.Invoke(runCommand.Error))
            {
                throw new(runCommand.Error);
            }
        }
        
        if (!string.IsNullOrWhiteSpace(runCommand.Result))
        {
            Log.Information("SSH result: {Error}", runCommand.Result);
        }
    }

    public static void RunSudo(this SshClient client, ISshOptions options, string command) =>
        client.RunCommand($"echo {options.SshPassword} | sudo -S {command}");
}