using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VUModManagerRegistry.Interfaces;
using VUModManagerRegistry.Models;

namespace VUModManagerRegistry.Services
{
    public class AccessTokenService : IAccessTokenService
    {
        private readonly RegistryContext _context;

        public AccessTokenService(RegistryContext context)
        {
            _context = context;
        }

        public async Task<Guid> Create()
        {
            var accessToken = new AccessToken()
            {
                Token = Guid.NewGuid()
            };

            await _context.AccessTokens.AddAsync(accessToken);
            await _context.SaveChangesAsync();

            return accessToken.Token;
        }

        public Task<bool> Verify(Guid accessToken)
        {
            return _context.AccessTokens.AnyAsync(t => t.Token == accessToken);
        }
    }
}