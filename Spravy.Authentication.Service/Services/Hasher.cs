using Spravy.Authentication.Domain.Interfaces;
using Spravy.Authentication.Service.Interfaces;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.Authentication.Service.Services;

public class Hasher : IHasher
{
    private readonly IBytesToString bytesToString;
    private readonly IHashService hashService;
    private readonly IStringToBytes stringToBytes;

    public Hasher(
        Ref<Named<IBytesToString>> bytesToString,
        Ref<Named<IHashService>> hashService,
        Ref<Named<IStringToBytes>> stringToBytes
    )
    {
        this.bytesToString = bytesToString.Value.Value;
        this.hashService = hashService.Value.Value;
        this.stringToBytes = stringToBytes.Value.Value;
        HashMethod = $"{stringToBytes.Value.Name};{hashService.Value.Name};{bytesToString.Value.Name}";
    }

    public string HashMethod { get; }

    public string ComputeHash(string input)
    {
        var bytes = stringToBytes.StringToBytes(input);
        var hashBytes = hashService.ComputeHash(bytes);
        var hash = bytesToString.BytesToString(hashBytes);

        return hash;
    }
}