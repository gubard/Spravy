namespace Spravy.Domain.Models;

public readonly struct RandomGuid : IRandom<Guid>
{
    public static readonly RandomGuid Default = new();

    public Guid GetRandom()
    {
        return Guid.NewGuid();
    }
}