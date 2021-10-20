using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VUModManagerRegistry.Models;
using VUModManagerRegistry.Repositories.Contracts;

namespace VUModManagerRegistry.Repositories
{
    public class ModUserPermissionRepository : IModUserPermissionRepository
    {
        private readonly AppDbContext _context;

        public ModUserPermissionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ModUserPermission> FindAsync(long modId, long userId, params string[] tags)
        {
            if (tags.Length > 0)
            {
                return await _context.Set<ModUserPermission>()
                    .FirstOrDefaultAsync(p => p.ModId == modId && p.UserId == userId && tags.Contains(p.Tag));
            }
            
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

        public async Task<bool> DeleteAsync(long modId, long userId, string tag)
        {
            var permission = await FindAsync(modId, userId, tag);
            if (permission == null)
            {
                return false;
            }

            _context.Set<ModUserPermission>().Remove(permission);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}