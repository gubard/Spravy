namespace Spravy.Authentication.Service.Services;

public class StringToBytesFactory : IFactory<string, Named<IStringToBytes>>
{
    public Result<Named<IStringToBytes>> Create(string key)
    {
        if (NamedHelper.StringToUtf8Bytes.Name == key)
        {
            return NamedHelper.StringToUtf8Bytes.ToResult();
        }

        return new(new NotFoundNamedError(key));
    }
}