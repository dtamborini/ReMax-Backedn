using System.Security.Claims;

public class UserClaimService
{
    public Guid? GetUserGuidFromClaims(IEnumerable<Claim> claims, string claimType = "guid")
    {
        if (claims == null)
        {
            return null;
        }

        var uuidClaim = claims.FirstOrDefault(c => c.Type == claimType);

        Guid? userGuid = null;
        if (uuidClaim != null && Guid.TryParse(uuidClaim.Value, out Guid parsedUuid))
        {
            userGuid = parsedUuid;
        }

        return userGuid;
    }

    public Guid? GetUserGuidFromPrincipal(ClaimsPrincipal userPrincipal, string claimType = "guid")
    {
        if (userPrincipal?.Claims == null)
        {
            return null;
        }
        return GetUserGuidFromClaims(userPrincipal.Claims, claimType);
    }
}