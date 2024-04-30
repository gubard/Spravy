using MailKit;
using MailKit.Net.Imap;
using MailKit.Security;
using Spravy.Integration.Tests.Models;

namespace Spravy.Integration.Tests.Extensions;

public static class ImapConnectionExtension
{
    public static void Execute(this ImapConnection connection, Action<ImapClient> action)
    {
        using var client = new ImapClient();

        try
        {
            client.Connect(connection.Host, 993, SecureSocketOptions.SslOnConnect);
            client.Authenticate(connection.Login, connection.Password);
            action.Invoke(client);
        }
        finally
        {
            client.Disconnect(true);
        }
    }

    public static TValue Execute<TValue>(this ImapConnection connection, Func<ImapClient, TValue> action)
    {
        using var client = new ImapClient();

        try
        {
            client.Connect(connection.Host, 993, SecureSocketOptions.SslOnConnect);
            client.Authenticate(connection.Login, connection.Password);

            return action.Invoke(client);
        }
        finally
        {
            client.Disconnect(true);
        }
    }

    public static string GetLastEmailText(this ImapConnection connection)
    {
        return connection.Execute(client =>
        {
            var inbox = client.Inbox;
            var i = 0;

            while (true)
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                i++;
                inbox.Open(FolderAccess.ReadWrite);

                if (inbox.Count == 0)
                {
                    inbox.Close();

                    if (i == 100)
                    {
                        throw new("Inbox timeout");
                    }

                    continue;
                }

                var message = inbox.GetMessage(0);

                return message.TextBody;
            }
        });
    }

    public static void ClearInbox(this ImapConnection connection)
    {
        connection.Execute(client =>
        {
            var inbox = client.Inbox;
            var i = 0;

            while (true)
            {
                i++;
                inbox.Open(FolderAccess.ReadWrite);

                if (inbox.Count == 0)
                {
                    inbox.Close();

                    return;
                }

                var trash = client.GetFolder("Trash");
                inbox.MoveTo(0, trash);
                inbox.Close();

                if (i == 100)
                {
                    return;
                }
            }
        });
    }
}