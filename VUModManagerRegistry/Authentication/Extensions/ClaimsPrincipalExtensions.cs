using System.Linq;
using System.Security.Claims;

namespace VUModManagerRegistry.Authentication.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Returns id of principal
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        public static long Id(this ClaimsPrincipal principal)
        {
            var id = principal.Claims.FirstOrDefault(c => c.Type == "Id");
            if (id == null)
            {
                return -1;
            }

            return long.Parse(id.Value);
        }
    }
}