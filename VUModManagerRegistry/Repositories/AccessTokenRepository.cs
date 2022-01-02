using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VUModManagerRegistry.Common.Interfaces;
using VUModManagerRegistry.Models;

namespace VUModManagerRegistry.Repositories
{
    public class AccessTokenRepository : RepositoryBase<UserAccessToken, AppDbContext>, IAccessTokenRepository
    {
        public AccessTokenRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<UserAccessToken>> FindAllByUserIdAsync(long userId)
        {
            return await Set.Where(t => t.UserId == userId).ToListAsync();
        }

        public async Task<UserAccessToken> FindByTokenAsync(Guid token)
        {
            return await Set.FirstOrDefaultAsync(t => t.Token == token);
        }

        public async Task<UserAccessToken> FindByUserIdAndTokenAsync(long userId, Guid token)
        {
            return await Set.FirstOrDefaultAsync(t => t.UserId == userId && t.Token == token);
        }
    }
}