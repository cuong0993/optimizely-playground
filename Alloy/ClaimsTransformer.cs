using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace Alloy;

public class ClaimsTransformer : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var claimsIdentity = new ClaimsIdentity();
        claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "WebAdmins"));
        claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "CmsAdmins"));

        principal.AddIdentity(claimsIdentity);

        return Task.FromResult(principal);
    }
}