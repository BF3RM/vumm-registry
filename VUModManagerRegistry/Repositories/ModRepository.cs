using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VUModManagerRegistry.Common.Interfaces;
using VUModManagerRegistry.Models;

namespace VUModManagerRegistry.Repositories
{
    public class ModRepository : RepositoryBase<Mod, AppDbContext>, IModRepository
    {
        public ModRepository(AppDbContext context) : base(context)
        {
        }

        public Task<Mod> FindByNameAsync(string name)
        {
            return Set.FirstOrDefaultAsync(m => m.Name == name);
        }

        public Task<Mod> FindByNameWithVersionsAsync(string name)
        {
            return Set
                .Include(m => m.Versions)
                .FirstOrDefaultAsync(m => m.Name == name);
        }

        public async Task<bool> DeleteByNameAsync(string name)
        {
            var mod = await FindByNameAsync(null);
            if (mod == null)
            {
                return false;
            }

            Set.Remove(mod);
            await Context.SaveChangesAsync();

            return true;
        }

        public async Task<ICollection<ModUserPermission>> FindPermissionsByIdAndUserIdAsync(long modId, long userId)
        {
            var mod = await Set
                .Include(m => m.UserPermissions
                    .Where(p => p.UserId == userId))
                .FirstOrDefaultAsync(m => m.Id == modId);

            return mod?.UserPermissions;
        }
    }
}