using System.Threading.Tasks;
using VUModManagerRegistry.Models;

namespace VUModManagerRegistry.Repositories.Contracts
{
    public interface IModUserPermissionRepository
    {
        Task<ModUserPermission> FindAsync(long modId, long userId);
        Task<ModUserPermission> FindAsync(long modId, long userId, string tag);
        Task<ModUserPermission> AddAsync(ModUserPermission permission);
        Task<ModUserPermission> UpdateAsync(ModUserPermission permission);
        Task<bool> DeleteByModAndUserIdAsync(long modId, long userId);
    }
}