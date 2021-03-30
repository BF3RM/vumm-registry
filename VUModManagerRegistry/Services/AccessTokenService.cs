using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VUModManagerRegistry.Models;
using VUModManagerRegistry.Repositories;
using VUModManagerRegistry.Repositories.Contracts;
using VUModManagerRegistry.Services.Contracts;

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

            return await _repository.DeleteAsync(accessToken.Id);
        }

        public async Task<List<UserAccessToken>> GetAll(long userId)
        {
            return await _repository.FindAllByUserIdAsync(userId);
        }
    }
}