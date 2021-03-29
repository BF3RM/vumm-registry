using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VUModManagerRegistry.Interfaces;
using VUModManagerRegistry.Models;

namespace VUModManagerRegistry.Services
{
    public class AccessTokenService : IAccessTokenService
    {
        private readonly IAppDbContext _context;

        public AccessTokenService(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<UserAccessToken> Create(User user, AccessTokenType type = AccessTokenType.ReadOnly)
        {
            var accessToken = new UserAccessToken()
            {
                UserId = user.Id,
                Token = Guid.NewGuid(),
                Type = type
            };

            await _context.AccessTokens.AddAsync(accessToken);
            await _context.SaveChangesAsync();

            return accessToken;
        }

        public async Task<(bool IsValid, User User)> Verify(Guid accessToken)
        {
            var token = await _context.AccessTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Token == accessToken);

            if (token == null)
            {
                return (false, null);
            }

            return (true, token.User);
        }
    }
}