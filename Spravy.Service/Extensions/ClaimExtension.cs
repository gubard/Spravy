namespace Spravy.Service.Extensions;

public static class ClaimExtension
{
    public static Claim GetClaim(this IEnumerable<Claim> claims, string type)
    {
        return claims.Single(x => x.Type == type);
    }

    public static Claim GetNameClaim(this IEnumerable<Claim> claims)
    {
        return claims.GetClaim(ClaimTypes.Name);
    }

    public static Claim GetNameIdentifierClaim(this IEnumerable<Claim> claims)
    {
        return claims.GetClaim(ClaimTypes.NameIdentifier);
    }

    public static Claim GetRoleClaim(this IEnumerable<Claim> claims)
    {
        return claims.GetClaim(ClaimTypes.Role);
    }
}