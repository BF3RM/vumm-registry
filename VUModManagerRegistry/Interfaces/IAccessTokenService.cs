using System;
using System.Threading.Tasks;
using VUModManagerRegistry.Models;

namespace VUModManagerRegistry.Interfaces
{
    public interface IAccessTokenService
    {
        public Task<UserAccessToken> Create(User user, AccessTokenType type = AccessTokenType.ReadOnly);
        public Task<(bool IsValid, User User)> Verify(Guid accessToken);
        
    }
}