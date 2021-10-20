using System.Threading.Tasks;
using VUModManagerRegistry.Models;

namespace VUModManagerRegistry.Repositories.Contracts
{
    public interface IModUserPermissionRepository
    {
        Task<ModUserPermission> FindAsync(long modId, long userId, params string[] tags);
        Task<ModUserPermission> AddAsync(ModUserPermission permission);
        Task<ModUserPermission> UpdateAsync(ModUserPermission permission);
        Task<bool> DeleteAsync(long modId, long userId, string tag);
    }
}