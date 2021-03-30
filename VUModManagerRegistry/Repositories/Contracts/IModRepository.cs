using System.Collections.Generic;
using System.Threading.Tasks;
using VUModManagerRegistry.Models;

namespace VUModManagerRegistry.Repositories.Contracts
{
    public interface IModRepository : IRepository<Mod>
    {
        Task<ICollection<ModUserPermission>> FindPermissionsByIdAndUserId(long modId, long userId);
    }
}