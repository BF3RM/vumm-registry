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
        /// <param name="tag">the version tag</param>
        /// <param name="permissions">the permissions to check</param>
        /// <returns></returns>
        Task<bool> HasAnyPermissions(long modId, long userId, params ModPermission[] permissions);
        Task<bool> HasAnyPermissions(long modId, long userId, string tag, params ModPermission[] permissions);
        
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
        /// <param name="modId">the mod id</param>
        /// <param name="userId">the user id</param>
        /// <param name="tag">the tag to set the permission for</param>
        /// <param name="permission">the permission to add</param>
        /// <returns></returns>
        Task<bool> SetPermission(long modId, long userId, string tag, ModPermission permission);

        /// <summary>
        /// Add mod permission to user
        /// </summary>
        /// <param name="modId">the mod id</param>
        /// <param name="username">the username</param>
        /// <param name="tag">the tag to set the permission for</param>
        /// <param name="permission">the permission to add</param>
        /// <returns></returns>
        Task<bool> SetPermission(long modId, string username, string tag, ModPermission permission);
        
        /// <summary>
        /// Revoke mod permissions of user
        /// </summary>
        /// <param name="modId">the mod id</param>
        /// <param name="userId">the user id</param>
        /// <param name="tag">the tag to revoke the permission for</param>
        /// <returns></returns>
        Task<bool> RevokePermissions(long modId, long userId, string tag);
        
        /// <summary>
        /// Revoke mod permissions of user
        /// </summary>
        /// <param name="modId">the mod id</param>
        /// <param name="username">the username</param>
        /// <param name="tag">the tag to revoke the permission for</param>
        /// <returns></returns>
        Task<bool> RevokePermissions(long modId, string username, string tag);
    }
}