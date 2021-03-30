using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VUModManagerRegistry.Models;

namespace VUModManagerRegistry.Repositories
{
    public class ModRepository : RepositoryBase<Mod, AppDbContext>, IModRepository
    {
        public ModRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<ICollection<ModUserPermission>> FindPermissionsByIdAndUserId(long modId, long userId)
        {
            var mod = await Set
                .Include(m => m.UserPermissions
                    .Where(p => p.UserId == userId))
                .FirstOrDefaultAsync(m => m.Id == modId);

            return mod?.UserPermissions;
        }
    }
}