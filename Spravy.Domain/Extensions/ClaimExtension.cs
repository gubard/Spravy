using System.Security.Claims;

namespace Spravy.Domain.Extensions;

public static class ClaimExtension
{
    public static Claim GetClaim(this IEnumerable<Claim> claims, string type)
    {
        var claimValues = claims.Where(x => x.Type == type).ToArray();

        if (claimValues.Length == 0)
        {
            throw new($"Not found claim {type}");
        }

        if (claimValues.Length > 1)
        {
            throw new($"Multi claims {type}");
        }

        return claimValues[0];
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

    public static string GetName(this IEnumerable<Claim> claims)
    {
        return claims.GetNameClaim().Value;
    }
}
