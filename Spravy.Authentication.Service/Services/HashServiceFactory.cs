namespace Spravy.Authentication.Service.Services;

public class HashServiceFactory : IFactory<string, Named<IHashService>>
{
    public Result<Named<IHashService>> Create(string key)
    {
        if (NamedHelper.Sha512Hash.Name == key)
        {
            return NamedHelper.Sha512Hash.ToResult();
        }

        return new(new NotFoundNamedError(key));
    }
}
