using Renci.SshNet;

namespace _build.Extensions;

public static class SshClientExtension
{
    public static void SafeRun(this SshClient client, string command)
    {
        using var runCommand = client.RunCommand(command);
    }
}