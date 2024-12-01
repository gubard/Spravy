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

    public Result<IHasher> Create(string key)
    {
        var values = key.Split(";");

        return stringToBytesFactory.Create(values[0])
           .IfSuccess(
                hashServiceFactory.Create(values[1]),
                bytesToStringFactory.Create(values[2]),
                (stringToBytes, hashService, bytesToString) =>
                    new Hasher(new(bytesToString), new(hashService), new(stringToBytes)).ToResult<IHasher>()
            );
    }
}