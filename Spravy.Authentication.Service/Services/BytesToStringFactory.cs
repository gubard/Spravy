namespace Spravy.Authentication.Service.Services;

public class BytesToStringFactory : IFactory<string, Named<IBytesToString>>
{
    public Result<Named<IBytesToString>> Create(string key)
    {
        if (NamedHelper.BytesToUpperCaseHexString.Name == key)
        {
            return NamedHelper.BytesToUpperCaseHexString.ToResult();
        }

        return new(new NotFoundNamedError(key));
    }
}