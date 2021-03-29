using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VUModManagerRegistry.Models;

namespace VUModManagerRegistry.Interfaces
{
    public interface IAccessTokenService
    {
        /// <summary>
        /// Create new access token for the user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public Task<UserAccessToken> Create(User user, AccessTokenType type = AccessTokenType.ReadOnly);
        
        /// <summary>
        /// Verify whether access token is valid and find it's user
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public Task<(bool IsValid, User User)> Verify(Guid accessToken);

        /// <summary>
        /// Revoke an access token
        /// </summary>
        /// <param name="userId">the user id</param>
        /// <param name="token">the token to revoke</param>
        /// <returns></returns>
        public Task<bool> Revoke(long userId, Guid token);

        /// <summary>
        /// Return all tokens owned by the user
        /// </summary>
        /// <param name="userId">the user id</param>
        /// <returns></returns>
        public Task<List<UserAccessToken>> GetAll(long userId);
    }
}