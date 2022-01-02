using System.Collections.Generic;
using System.Threading.Tasks;
using VUModManagerRegistry.Models;

namespace VUModManagerRegistry.Common.Interfaces
{
    public interface IModRepository : IRepository<Mod>
    {
        /// <summary>
        /// Find mod by name
        /// </summary>
        /// <param name="name">the mod name</param>
        /// <returns></returns>
        Task<Mod> FindByNameAsync(string name);
        
        /// <summary> 
        /// Find mod by name and join mod versions
        /// </summary>
        /// <param name="name">the mod name</param>
        /// <returns></returns>
        Task<Mod> FindByNameWithVersionsAsync(string name);

        /// <summary>
        /// Delete mod by its name
        /// </summary>
        /// <param name="name">the mod name</param>
        /// <returns></returns>
        Task<bool> DeleteByNameAsync(string name);
        
        /// <summary>
        /// Find correlated mod user permissions by id and user id
        /// </summary>
        /// <param name="modId">The mod id</param>
        /// <param name="userId">The user id</param>
        /// <returns></returns>
        Task<ICollection<ModUserPermission>> FindPermissionsByIdAndUserIdAsync(long modId, long userId);
    }
}