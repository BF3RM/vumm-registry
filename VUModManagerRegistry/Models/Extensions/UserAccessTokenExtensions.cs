using System.Collections.Generic;
using System.Linq;
using VUModManagerRegistry.Models.Dtos;

namespace VUModManagerRegistry.Models.Extensions
{
    public static class UserAccessTokenExtensions
    {
        /// <summary>
        /// Convert UserAccessToken to UserAccessTokenDto
        /// </summary>
        /// <param name="userAccessToken"></param>
        /// <returns></returns>
        public static UserAccessTokenDto ToDto(this UserAccessToken userAccessToken)
            => new()
            {
                Type = userAccessToken.Type,
                Token = userAccessToken.Token,
                CreatedAt = userAccessToken.Created
            };

        /// <summary>
        /// Convert enumerable of user access tokens to list of dto's
        /// </summary>
        /// <param name="userAccessTokens"></param>
        /// <returns></returns>
        public static List<UserAccessTokenDto> ToDtoList(this IEnumerable<UserAccessToken> userAccessTokens)
            => userAccessTokens.Select(t => t.ToDto()).ToList();
    }
}