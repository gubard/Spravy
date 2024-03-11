namespace _build.Models;

public class ServiceProjectBuilderOptions
{
    public ServiceProjectBuilderOptions(ushort port, string token, string emailPassword)
    {
        Port = port;
        Token = token;
        EmailPassword = emailPassword;
    }

    public ushort Port { get; }
    public string Token { get; }
    public string EmailPassword { get; }
}