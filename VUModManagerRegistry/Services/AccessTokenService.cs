using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VUModManagerRegistry.Interfaces;
using VUModManagerRegistry.Models;

namespace VUModManagerRegistry.Services
{
    public class AccessTokenService : IAccessTokenService
    {
        private readonly AppDbContext _context;

        public AccessTokenService(AppDbContext context)
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

        public async Task<bool> Revoke(long userId, Guid token)
        {
            var accessToken = await _context.AccessTokens.FirstOrDefaultAsync(t => t.UserId == userId && t.Token == token);
            if (accessToken == null)
            {
                return false;
            }
            
            _context.AccessTokens.Remove(accessToken);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<UserAccessToken>> GetAll(long userId)
        {
            return await _context.AccessTokens.Where(t => t.UserId == userId).ToListAsync();
        }
    }
}