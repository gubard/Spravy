using Spravy.Authentication.Domain.Interfaces;
using Spravy.Authentication.Service.Interfaces;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.Authentication.Service.Services;

public class HasherFactory : IFactory<string, IHasher>
{
    private readonly IFactory<string, Named<IBytesToString>> bytesToStringFactory;
    private readonly IFactory<string, Named<IHashService>> hashServiceFactory;
    private readonly IFactory<string, Named<IStringToBytes>> stringToBytesFactory;

    public HasherFactory(
        IFactory<string, Named<IBytesToString>> bytesToStringFactory,
        IFactory<string, Named<IHashService>> hashServiceFactory,
        IFactory<string, Named<IStringToBytes>> stringToBytesFactory
    )
    {
        this.bytesToStringFactory = bytesToStringFactory;
        this.hashServiceFactory = hashServiceFactory;
        this.stringToBytesFactory = stringToBytesFactory;
    }

    public IHasher Create(string key)
    {
        var values = key.Split(";");
        var stringToBytes = stringToBytesFactory.Create(values[0]);
        var hashService = hashServiceFactory.Create(values[1]);
        var bytesToString = bytesToStringFactory.Create(values[2]);

        return new Hasher(
            new Ref<Named<IBytesToString>>(bytesToString),
            new Ref<Named<IHashService>>(hashService),
            new Ref<Named<IStringToBytes>>(stringToBytes)
        );
    }
}