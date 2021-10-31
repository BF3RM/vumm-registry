using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VUModManagerRegistry.Common.Interfaces;
using VUModManagerRegistry.Models;

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
            var query = @"
                SELECT MV.* FROM ""ModVersions"" as MV
                LEFT JOIN ""ModUserPermissions"" as MUP
                    ON MUP.""ModId"" = MV.""ModId""
                    AND CASE
                        WHEN MUP.""Tag"" = ''
                            THEN 1
                        WHEN MUP.""Tag"" = MV.""Tag""
                            THEN 1
                        ELSE 0
                    END = 1
                WHERE MUP.""UserId"" = {1} AND MV.""Name"" = {0}
            ";

            return await Set.FromSqlRaw(query, name, userId).ToListAsync();
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