using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VUModManagerRegistry.Models;

namespace VUModManagerRegistry.Common.Interfaces
{
    public interface IAccessTokenService
    {
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