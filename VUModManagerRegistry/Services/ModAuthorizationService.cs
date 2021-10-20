using System.Linq;
using System.Threading.Tasks;
using VUModManagerRegistry.Models;
using VUModManagerRegistry.Repositories.Contracts;
using VUModManagerRegistry.Services.Contracts;

namespace VUModManagerRegistry.Services
{
    public class ModAuthorizationService : IModAuthorizationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IModUserPermissionRepository _permissionRepository;

        public ModAuthorizationService(
            IModUserPermissionRepository permissionRepository, IUserRepository userRepository)
        {
            _permissionRepository = permissionRepository;
            _userRepository = userRepository;
        }

        public async Task<bool> HasAnyPermissions(long modId, long userId, params ModPermission[] permissions)
        {
            var userPermission = await _permissionRepository.FindAsync(modId, userId);
            return userPermission != null && permissions.Contains(userPermission.Permission);
        }

        public async Task<bool> HasAnyPermissions(long modId, long userId, string tag, params ModPermission[] permissions)
        {
            var userPermission = await _permissionRepository.FindAsync(modId, userId, tag, "");
            return userPermission != null && permissions.Contains(userPermission.Permission);
        }

        public async Task<bool> SetPermission(long modId, long userId, string tag, ModPermission permission)
        {
            var userPermission = await _permissionRepository.FindAsync(modId, userId, tag);
            if (userPermission == null)
            {
                userPermission = new ModUserPermission()
                {
                    ModId = modId,
                    UserId = userId,
                    Permission = permission,
                    Tag = tag ?? ""
                };
                await _permissionRepository.AddAsync(userPermission);
                return true;
            }

            if (userPermission.Permission != permission)
            {
                userPermission.Permission = permission;
                await _permissionRepository.UpdateAsync(userPermission);

                return true;
            }

            return false;
        }

        public async Task<bool> SetPermission(long modId, long userId, ModPermission permission)
        {
            return await SetPermission(modId, userId, "", permission);
        }

        public async Task<bool> SetPermission(long modId, string username, string tag, ModPermission permission)
        {
            var user = await _userRepository.FindByUsernameAsync(username);
            if (user == null)
            {
                return false;
            }

            return await SetPermission(modId, user.Id, tag, permission);
        }

        public Task<bool> RevokePermissions(long modId, long userId, string tag = "")
        {
            return _permissionRepository.DeleteAsync(modId, userId, tag ?? "");
        }

        public async Task<bool> RevokePermissions(long modId, string username, string tag)
        {
            var user = await _userRepository.FindByUsernameAsync(username);
            if (user == null)
            {
                return false;
            }

            return await RevokePermissions(modId, user.Id, tag);
        }
    }
}