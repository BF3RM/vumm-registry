using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VUModManagerRegistry.Models;
using VUModManagerRegistry.Repositories.Contracts;

namespace VUModManagerRegistry.Repositories
{
    public class ModUserPermissionRepository : IModUserPermissionRepository
    {
        private AppDbContext _context;

        public ModUserPermissionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ModUserPermission> FindByModAndUserIdAsync(long modId, long userId)
        {
            return await _context.Set<ModUserPermission>()
                .FirstOrDefaultAsync(p => p.ModId == modId && p.UserId == userId);
        }

        public async Task<ModUserPermission> AddAsync(ModUserPermission permission)
        {
            await _context.Set<ModUserPermission>().AddAsync(permission);
            await _context.SaveChangesAsync();
            return permission;
        }

        public async Task<ModUserPermission> UpdateAsync(ModUserPermission permission)
        {
            _context.Entry(permission).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return permission;
        }

        public Task<ModUserPermission> DeleteByModAndUserIdAsync(long modId, long userId)
        {
            throw new System.NotImplementedException();
        }
    }
}