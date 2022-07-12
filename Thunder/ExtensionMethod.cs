using System.Security.Claims;

namespace Thunder
{
    public static class ExtensionMethod
    {
        public static object GetClaim(this ClaimsPrincipal user, string type)
        {
            List<Claim> claims = user.Claims.ToList();
            var claimValue = claims.Where(claim => claim.Type.Contains(type)).FirstOrDefault().Value;
            return claimValue != null ? claimValue : "";
        }

        public static string GetId(this ClaimsPrincipal user)
        {
            return GetClaim(user, "Id").ToString();
        }

        public static string GetRoleId(this ClaimsPrincipal user)
        {
            return GetClaim(user, "RoleId").ToString();
        }

        public static string GetName(this ClaimsPrincipal user)
        {
            return GetClaim(user, "Name").ToString();
        }

        public static string GetEmail(this ClaimsPrincipal user)
        {
            return GetClaim(user, "Email").ToString();
        }

    }
}
