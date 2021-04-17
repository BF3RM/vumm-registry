using System.Threading.Tasks;
using VUModManagerRegistry.Models;

namespace VUModManagerRegistry.Services.Contracts
{
    public interface IModAuthorizationService
    {
        /// <summary>
        /// Checks whether user has any of the given permissions
        /// </summary>
        /// <param name="modId">the mod id</param>
        /// <param name="userId">the user id</param>
        /// <param name="permissions">the permissions to check</param>
        /// <returns></returns>
        Task<bool> HasAnyPermissions(long modId, long userId, params ModPermission[] permissions);
        
        /// <summary>
        /// Add mod permission to user
        /// </summary>
        /// <param name="modId">the mod id</param>
        /// <param name="userId">the user id</param>
        /// <param name="permission">the permission to add</param>
        /// <returns></returns>
        Task<bool> SetPermission(long modId, long userId, ModPermission permission);

        /// <summary>
        /// Add mod permission to user
        /// </summary>
        /// <param name="modId"></param>
        /// <param name="username"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        Task<bool> SetPermission(long modId, string username, ModPermission permission);
        
        /// <summary>
        /// Revoke mod permissions of user
        /// </summary>
        /// <param name="modId">the mod id</param>
        /// <param name="userId">the user id</param>
        /// <returns></returns>
        Task<bool> RevokePermissions(long modId, long userId);
        
        /// <summary>
        /// Revoke mod permissions of user
        /// </summary>
        /// <param name="modId">the mod id</param>
        /// <param name="username">the username</param>
        /// <returns></returns>
        Task<bool> RevokePermissions(long modId, string username);
    }
}