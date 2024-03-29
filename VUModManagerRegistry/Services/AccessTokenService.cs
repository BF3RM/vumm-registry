using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VUModManagerRegistry.Common.Interfaces;
using VUModManagerRegistry.Models;

namespace VUModManagerRegistry.Services
{
    public class AccessTokenService : IAccessTokenService
    {
        private readonly IAccessTokenRepository _repository;

        public AccessTokenService(IAccessTokenRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Revoke(long userId, Guid token)
        {
            var accessToken = await _repository.FindByUserIdAndTokenAsync(userId, token);
            if (accessToken == null)
            {
                return false;
            }

            return await _repository.DeleteByIdAsync(accessToken.Id);
        }

        public async Task<List<UserAccessToken>> GetAll(long userId)
        {
            return await _repository.FindAllByUserIdAsync(userId);
        }
    }
}