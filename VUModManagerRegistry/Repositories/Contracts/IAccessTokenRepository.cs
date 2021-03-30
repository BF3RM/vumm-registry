using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VUModManagerRegistry.Models;

namespace VUModManagerRegistry.Repositories.Contracts
{
    public interface IAccessTokenRepository : IRepository<UserAccessToken>
    {
        Task<List<UserAccessToken>> FindAllByUserIdAsync(long userId);

        Task<UserAccessToken> FindByTokenAsync(Guid token);
        
        Task<UserAccessToken> FindByUserIdAndTokenAsync(long userId, Guid token);
    }
}