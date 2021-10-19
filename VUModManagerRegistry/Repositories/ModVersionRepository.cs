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