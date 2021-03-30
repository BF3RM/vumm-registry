using System.Security.Claims;

namespace VUModManagerRegistry.Authentication.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Converts IIdentity to UserIdentity
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        public static UserIdentity AuthenticatedUser(this ClaimsPrincipal principal)
        {
            if (principal.Identity != null && principal.Identity.IsAuthenticated == false)
            {
                return null;
            }
            return principal.Identity as UserIdentity;
        }
    }
}