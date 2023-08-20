namespace Spravy.Authentication.Domain.Interfaces;

public interface IHashService
{
    byte[] ComputeHash(byte[] input);
}