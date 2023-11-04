using Renci.SshNet;
using Serilog;

namespace _build.Extensions;

public static class SshClientExtension
{
    public static void SafeRun(this SshClient client, string command)
    {
        using var runCommand = client.RunCommand(command);
        Log.Logger.Information("Run SSH command:{Command}", command);

        if (!string.IsNullOrWhiteSpace(runCommand.Error))
        {
            Log.Error("SSH Error: {Error}", runCommand.Error);
        }

        if (!string.IsNullOrWhiteSpace(runCommand.Result))
        {
            Log.Error("SSH result: {Error}", runCommand.Result);
        }
    }
}