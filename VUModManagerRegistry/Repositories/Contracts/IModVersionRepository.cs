using System.Collections.Generic;
using System.Threading.Tasks;
using VUModManagerRegistry.Models;

namespace VUModManagerRegistry.Repositories.Contracts
{
    public interface IModVersionRepository : IRepository<ModVersion>
    {
        Task<ModVersion> FindByNameAndVersion(string name, string version);

        Task<List<ModVersion>> FindAllowedVersions(string name, long userId);
        
        Task<bool> ExistsByNameAndVersionAsync(string name, string version);

        Task<bool> DeleteByNameAndVersionAsync(string name, string version);
    }
}