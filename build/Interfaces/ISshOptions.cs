namespace _build.Interfaces;

public interface ISshOptions
{
    string SshHost { get; }
    string SshUser { get; }
    string SshPassword { get; }
}