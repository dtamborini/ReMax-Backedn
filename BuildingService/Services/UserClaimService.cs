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

    public string? GetUserIdFromJwt(ClaimsPrincipal userPrincipal)
    {
        if (userPrincipal?.Claims == null)
        {
            return null;
        }

        // Try different claim types used by ExternalAuth JWT
        var userIdClaim = userPrincipal.Claims.FirstOrDefault(c => 
            c.Type == "sub" || 
            c.Type == "user_id" || 
            c.Type == "guid" ||
            c.Type == ClaimTypes.NameIdentifier);

        return userIdClaim?.Value;
    }

    public Guid? GetUserGuidFromJwt(ClaimsPrincipal userPrincipal)
    {
        var userId = GetUserIdFromJwt(userPrincipal);
        if (userId != null && Guid.TryParse(userId, out Guid userGuid))
        {
            return userGuid;
        }
        return null;
    }
}