namespace _build.Interfaces;

public class SshOptions : ISshOptions
{
    public SshOptions(string sshHost, string sshUser, string sshPassword)
    {
        SshHost = sshHost;
        SshUser = sshUser;
        SshPassword = sshPassword;
    }

    public string SshHost { get; }
    public string SshUser { get; }
    public string SshPassword { get; }
}