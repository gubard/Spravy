namespace Spravy.Authentication.Service.Helpers;

public static class NamedHelper
{
    public static readonly Named<IHashService> Sha512Hash = new("SHA512", new Sha512HashService());
    public static readonly Named<IStringToBytes> StringToUtf8Bytes =
        new("UTF8", new StringToUtf8Bytes());

    public static readonly Named<IBytesToString> BytesToUpperCaseHexString =
        new("UpperCaseHex", new BytesToUpperCaseHexString());
}
