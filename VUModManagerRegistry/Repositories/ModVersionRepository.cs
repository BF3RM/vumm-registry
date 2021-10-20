using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VUModManagerRegistry.Models;
using VUModManagerRegistry.Repositories.Contracts;

namespace VUModManagerRegistry.Repositories
{
    public class ModVersionRepository : RepositoryBase<ModVersion, AppDbContext>, IModVersionRepository
    {
        public ModVersionRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<ModVersion> FindByNameAndVersion(string name, string version)
        {
            return await Set
                .Include(m => m.Mod)
                .FirstOrDefaultAsync(m => m.Name == name && m.Version == version);
        }
        
        public async Task<List<ModVersion>> FindAllowedVersions(string name, long userId)
        {
            var versions = from v in Set
                join m in Context.Mods on v.ModId equals m.Id
                from p in Context.ModUserPermissions
                where !m.IsPrivate || v.ModId == p.ModId && (v.Tag == p.Tag || p.Tag == "")
                where v.Name == name && p.UserId == userId
                select v;

            return await versions.ToListAsync();
        }

        public async Task<bool> ExistsByNameAndVersionAsync(string name, string version)
        {
            return await Set.AnyAsync(m => m.Name == name && m.Version == version);
        }

        public async Task<bool> DeleteByNameAndVersionAsync(string name, string version)
        {
            var modVersion = await FindByNameAndVersion(name, version);
            if (modVersion == null)
            {
                return false;
            }

            Set.Remove(modVersion);
            await Context.SaveChangesAsync();

            return true;
        }
    }
}